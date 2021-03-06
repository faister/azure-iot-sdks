// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

'use strict';

var Message = require('./message.js');

/**
 * @class module:azure-iot-common.Http
 * @classdesc Basic HTTP request/response functionality used by higher-level IoT Hub libraries.
 */
function Http() {
  this._https = require('https');
}

/*Codes_SRS_NODE_HTTP_05_001: [buildRequest shall accept the following arguments:
method - an HTTP verb, e.g., 'GET', 'POST', 'DELETE'
path - the path to the resource, not including the hostname
httpHeaders - an object whose properties represent the names and values of HTTP headers to include in the request
host - the fully-qualified DNS hostname of the IoT hub
done - a callback that will be invoked when a completed response is returned from the server]*/
Http.prototype.buildRequest = function (method, path, httpHeaders, host, done) {
  var options = {
    host: host,
    path: path,
    method: method,
    headers: httpHeaders
  };

  var request = this._https.request(options, function onResponse(response) {
    var responseBody = '';
    response.on('data', function onResponseData(chunk) {
      responseBody += chunk;
    });
    response.on('end', function onResponseEnd() {
      /*Codes_SRS_NODE_HTTP_05_005: [When an HTTP response is received, the callback function indicated by the done argument shall be invoked with the following arguments:
      err - the standard JavaScript Error object if status code >= 300, otherwise null
      body - the body of the HTTP response as a string
      response - the Node.js http.ServerResponse object returned by the transport]*/
      var err = (response.statusCode >= 300) ?
        new Error(response.statusMessage) :
        null;
      done(err, responseBody, response);
    });
  });

  /*Codes_SRS_NODE_HTTP_05_003: [If buildRequest encounters an error before it can send the request, it shall invoke the done callback function and pass the standard JavaScript Error object with a text description of the error (err.message).]*/
  request.on('error', done);
  /*Codes_SRS_NODE_HTTP_05_002: [buildRequest shall return a Node.js https.ClientRequest object, upon which the caller must invoke the end method in order to actually send the request.]*/
  return request;
};

Http.prototype.toMessage = function toMessage(response, body) {
  var msg;
  /*Codes_SRS_NODE_HTTP_05_006: [If the status code of the HTTP response < 300, toMessage shall create a new azure-iot-common.Message object with data equal to the body of the HTTP response.]*/
  if (response.statusCode < 300) {
    msg = new Message(body);
    for (var item in response.headers) {
      if (item.search("iothub-") !== -1) {
        /*Codes_SRS_NODE_HTTP_05_007: [If the HTTP response has an 'iothub-messageid' header, it shall be saved as the messageId property on the created Message.]*/
        if (item.toLowerCase() === "iothub-messageid") {
          msg.messageId = response.headers[item];
        }
        /*Codes_SRS_NODE_HTTP_05_008: [If the HTTP response has an 'iothub-to' header, it shall be saved as the to property on the created Message.]*/
        else if (item.toLowerCase() === "iothub-to") {
          msg.to = response.headers[item];
        }
        /*Codes_SRS_NODE_HTTP_05_009: [If the HTTP response has an 'iothub-expiry' header, it shall be saved as the expiryTimeUtc property on the created Message.]*/
        else if (item.toLowerCase() === "iothub-expiry") {
          msg.expiryTimeUtc = response.headers[item];
        }
        /*Codes_SRS_NODE_HTTP_05_010: [If the HTTP response has an 'iothub-correlationid' header, it shall be saved as the correlationId property on the created Message.]*/
        else if (item.toLowerCase() === "iothub-correlationid") {
          msg.correlationId = response.headers[item];
        }
      }
      /*Codes_SRS_NODE_HTTP_05_011: [If the HTTP response has an 'etag' header, it shall be saved as the lockToken property on the created Message, minus any surrounding quotes.]*/
      else if (item.toLowerCase() === "etag") {
        // Need to strip the quotes from the string
        var len = response.headers[item].length;
        msg.lockToken = response.headers[item].substring(1, len-1);
      }
    }
  }

  return msg;
};

module.exports = Http;

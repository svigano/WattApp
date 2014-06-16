var express = require('express');
var fs = require('fs');
var cors = require('cors');
var app = express();

app.use(cors());

process.env.NODE_TLS_REJECT_UNAUTHORIZED = "0";

/* Meters */
app.get('/wattappAPI/mocks/meters', function(req, res){

	data  = require('./metersData.json');
	res.send(data);
});


app.listen(8080); //to port on which the express server listen
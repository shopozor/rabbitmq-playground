'use strict'

const bodyParser = require('body-parser')
const {
  spike
} = require('./functions')

module.exports = ({ app }, wrap) => {
  app.use(bodyParser.json())
  app.post('/', wrap(spike))
}

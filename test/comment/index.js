#!/usr/bin/env node

const bot = require("circle-github-bot").create();
const fs = require('fs');

const path = '../logger/Output.log'
fs.readFile(path, (err, data) => {
    if(err) {
        console.log(`Fail to read file: ${path}, err: ${err.message}`)
    }
    if(data.length > 0) {
        console.log(data.toString())
        bot.comment(`
        <h3>Warnings</h3>
        ${data}
        `);
    } else {
        console.log('no warnings or errors')
    }
})


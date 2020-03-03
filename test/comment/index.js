const bot = require("circle-github-bot").create();
const fs = require('fs');
const { getArtifactByPath } = require('./artifact')

const path = '../result/Output.log'
fs.readFile(path, (err, data) => {
    if (err) {
        console.log(`Fail to read file: ${path}, err: ${err.message}`)
    }
    const regex = /(?<errors>\d*) Error\(s\), (?<warnings>\d*) Warning\(s\)/gm
    const { errors, warnings } = regex.exec(data.toString()).groups
    getArtifactByPath('test/result/Output.log').then((artifactUrl) =>{
        bot.comment(process.env.GH_AUTH_TOKEN, `<h3>Errors: ${errors}, Warnings: ${warnings}</h3>
    Details: <strong><a href='${artifactUrl}' target='_blank'>details</a></strong>
    `);
    }).catch(error => {
        console.log(`Fail to fetch the artifact url for output.log, err: ${error.message}`)
    })
    
})


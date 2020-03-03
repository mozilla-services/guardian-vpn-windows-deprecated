const request = require('request');

const getArtifactByPath = (path) => {
    const options = {
        'method': 'GET',
        'url': `https://circleci.com/api/v2/project/gh/mozilla-services/guardian-vpn-windows/${process.env.CIRCLE_BUILD_NUM}/artifacts`,
        'headers': {
            'Circle-Token': process.env.CIRCLECI_API_TOKEN,
            'Accept': 'application/json'
        }
    };
    return new Promise((res, rej) => {
        request(options, (error, response) => {
            if (error) rej(error)
            const artifact =  JSON.parse(response.body).items.find(item => item.path === path)
            res(artifact.url)
        })
    })
}

module.exports = {
    getArtifactByPath
}
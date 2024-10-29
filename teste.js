const axios = require('axios');

axios.get('https://crqsp.org.br/os-cuidados-com-a-beleza/')
  .then(response => {
    console.log('Status:', response.status);
  })
  .catch(error => {
    console.error('Error:', error.message);
    if (error.response) {
      console.error('Status:', error.response.status);
      console.error('Headers:', error.response.headers);
    }
  });

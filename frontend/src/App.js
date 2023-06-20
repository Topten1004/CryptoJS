import logo from './logo.svg';
import './App.css';
import axios from 'axios'
import React, { useState } from 'react';
import CryptoJS from 'crypto-js';
import Button from '@mui/material/Button';
import Snackbar, { SnackbarOrigin } from '@mui/material/Snackbar';
import Alert from '@mui/material/Alert';

const App = () => {
  const [userId, setUserId] = useState('');
  const [password, setPassword] = useState('');
  const [flag, setFlag] = useState(false);

  const handleClose = () => {
    setFlag(false);
  }

  const handleLogin = async () => {
    
    var key = CryptoJS.enc.Utf8.parse('8056483646328763');
    var iv = CryptoJS.enc.Utf8.parse('8056483646328763');

    var encrypted = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(password), key,
    {
    keySize: 128 / 8,
    iv: iv,
    mode: CryptoJS.mode.CBC,
    padding: CryptoJS.pad.Pkcs7
    }).toString();
        
    try {

      console.log(encrypted);
      const response = await axios.post('https://localhost:7009/User/Login', {
        userId : userId,
        password : encrypted,
      }, {
        headers: {
          'Content-Type': 'application/json',
        },
      });

      // Handle successful login
      console.log("response: ", response);

      if(response.data.status === 1)
      {
        setFlag(true);
      }
      alert(response.data.message);

    } catch (error) {
      // Handle login error
      alert("Can't login");
      console.error(error);
    }
  };

  return (
    <div className="flex flex-col justify-center items-center h-screen">
        <div className="mb-4">
          <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="userId">
            User ID
          </label>
          <Snackbar
            anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
            open={flag}
            onClose={handleClose}
          >
            <Alert severity="error">This is an information message!</Alert>
          </Snackbar>
          <input
            className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            id="userId"
            type="text"
            placeholder="Enter your user ID"
            value={userId}
            onChange={(e) => setUserId(e.target.value)}
          />
        </div>
        <div className="mb-4">
          <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="password">
            Password
          </label>
          <input
            className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 mb-3 leading-tight focus:outline-none focus:shadow-outline"
            id="password"
            type="password"
            placeholder="Enter your password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
        </div>
        <div className="flex items-center justify-center">
          <button
            className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
            onClick={handleLogin}
          >
            Log In
          </button>
        </div>
    </div>
  );
};

export default App;
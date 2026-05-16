In order to run app u need to specify JWT Settings in **appsettings** or **Secrets**.
Format:
  "Jwt": {
    "Key": "{kluchick}",
    "Issuer": "http://localhost",
    "Audience": "http://localhost",
    "ExpireDays": "7"
  }

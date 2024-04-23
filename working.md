# eShop OnWeb

A fork of eShopOnWeb with vulnerable code paths.

## Starting the application with Contrast

`Contrast.SensorsNetCore` nuget package has been added to the `Web` project. Launch profiles have been added to `launchSettings.json`.

Start application on Windows:

1. Select `Web` as startup application (default) -> Select `Web-Contrast-Windows` as startup profile
1. `dotnet run --launch-profile Web-Contrast-Windows`

Start application on Ubuntu:

1. Select `Web` as startup application (default) -> Select `Web-Contrast-Ubuntu` as startup profile
1. `dotnet run --launch-profile Web-Contrast-Ubuntu`

## Vulnerable Paths

### Unvalidated Redirect

Discover:

- https://localhost:5001/
- Navigate to "LOGIN"
- Click "Register as a new user" (notice `returnUrl=%2F` in the browser URL)
- Fill out with valid email and password (6+ characters, one non-alpha, one lowercase, one uppercase, ugh)
- Click "Register" button
- Check logs for `unvalidated-redirect`

Exploit:

- https://localhost:5001/Identity/Account/Register?returnUrl=http%3A%2F%2Fwww.google.com

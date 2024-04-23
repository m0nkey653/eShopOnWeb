# eShop OnWeb

A fork of eShopOnWeb with vulnerable code paths.

## Starting the application with Contrast

`Contrast.SensorsNetCore` nuget package has been added to the `Web` project. Launch profiles have been added to `launchSettings.json`.

Start application on Windows:

1. Select `Web` as startup application (default) -> Select `Web-Contrast-Windows` as startup profile
1. `dotnet run --launch-profile Web-Contrast-Windows`

Start application on Ubuntu:

1. Install `dotnet-sdk-8.0` - [Install .NET SDK or .NET Runtime on Ubuntu](https://learn.microsoft.com/en-us/dotnet/core/install/linux-ubuntu-install?tabs=dotnet8&pivots=os-linux-ubuntu-2404)
1. In a console, navigate to `eShopOnWeb/src/Web/`
1. `dotnet run --launch-profile Web-Contrast-Ubuntu`

## Vulnerable Paths

### Unvalidated Redirect

Vuln JSON: [unvalidated-redirect.json](./vulns/unvalidated-redirect.json)

Discover:

- https://localhost:5001/
- Navigate to "LOGIN"
- Click "Register as a new user" (notice `returnUrl=%2F` in the browser URL)
- Fill out with valid email and password (6+ characters, one non-alpha, one lowercase, one uppercase, ugh)
- Click "Register" button
- Check logs for `unvalidated-redirect`

Exploit:

- https://localhost:5001/Identity/Account/Register?returnUrl=http%3A%2F%2Fwww.google.com

Fix:

[Register.cshtml.cs](./src/Web/Areas/Identity/Pages/Account/Register.cshtml.cs)

```csharp
    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/");
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser { UserName = Input?.Email, Email = Input?.Email };
            var result = await _userManager.CreateAsync(user, Input?.Password!);
            if (result.Succeeded)
            {
...
...
                // Vulnerable: Redirect can go anywhere
                // return Redirect(returnUrl);

                // Safe: enforces a local redirect
                return LocalRedirect(returnUrl);
```

### Reflected XSS

Vuln JSON: [reflected-xss.json](./vulns/reflected-xss.json)

Discover:

- https://localhost:5001/order/search/term/invalidordernumber

Exploit:

- https://localhost:5001/order/search/term/test%3Cimg%20src%3Dx%20onerror%3Dalert(0)%3E 

Fix:

[Search.cshtml](./src/Web/Views/Order/Search.cshtml)

```html
    <p>
        @* Vulnerable code below with Html.Raw *@
        @* Could not find order for: "@Html.Raw(Model.SearchTerm)". *@
        
        @* Safe code below with default html encoding *@
        Could not find order for: "@Model.SearchTerm".
    </p>
```
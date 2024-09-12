# Adita.PlexNet.WinUI.Authorization

This library is designed to work with:
- `Adita.PlexNet.Core.Identity`
- `Adita.PlexNet.Core.Identity.EntityFrameworkCore`
- `Adita.PlexNet.Core.Security`

You can learn about The PlexNet Identity [here](https://github.com/sans-eng/Adita.PlexNet.Core.Identity).

## How to use

In the constructor of your `App.xaml.cs` class:

```
 //Set thread principal to anonymous before executing any codes
 Thread.CurrentPrincipal = new Adita.PlexNet.Core.Security.Principals.ApplicationPrincipal(new Adita.PlexNet.Core.Security.Claims.ApplicationIdentity());
```

Then you ready to use the behavior class in any `UIElement`, e.g.:
```
<Button>Click Me
    <i:Interaction.Behaviors>
        <authBehavior:AuthorizeBehavior Roles="Administrator;Maintainer"/>
    </i:Interaction.Behaviors>
</Button>
```
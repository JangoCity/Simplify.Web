### For examples see: https://github.com/i4004/AcspNet.Examples


Advanced Controls Site Platform .NET is an ASP.NET based web-sites plugin-based engine.
It is allows you to construct your web-site from a set of extensions (plugins). Each web site extension can do their own task.

* It is based on basic ASP.NET functionality (empty ASP.NET web-page);
* Web-site is constructing from extensions (plugins);
* Extensions can contain some functionality shared between other extensions or can be executed depending on HTTP query string parameters and do some web-page build etc.;
* It is NOT using ASP.NET controls and include their own fast web-page render;
* Starting from version 2.0 you can set extension parameters via class attributes.

AcspNet have two types of extensions:
* Executable (exec) extensions which can run depending on HTTP query string parameters only;
* Library (lib) extensions which can be used by other lib or exec extensions.

Recommended extrensions folder structure:

```text
YourProject
  -Extensions
    -Executable
      -Extension1.cs
      -Extension2.cs
    -Library
      -Extension3.cs
      -Extension4.cs
```

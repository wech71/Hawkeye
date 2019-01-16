[EDIT: 2019/01/16]
Forked from https://archive.codeplex.com/?p=hawkeye and modified for compilation with VisualStudio 2017 (C++/cli instead of clr:OldStyle and fix for .Net 4 instead of .Net 2)


[EDIT: 2010/01/10] In the case you are running an x86 Windows; you are greatly advised to upgrade to Hawkeye 1.2.5: the previous release is broken on these OS.
I apologize for the inconvenience, but it appears Hawkeye 1.2.4 (and probably previous versions) doesn't run properly on x86 Windows (See issue #7791). Hawkeye 1.2.5 fixes this issue.

Project Description
Debugging a managed Windows application is, most of the time, not an easy task. Thus, any tool that can help will make your life easier.

Hawkeye is the only .Net tool that allows you to view, edit, analyze and invoke (almost) any object from a .Net application. Whenever you try to debug, test, change or understand an application, Hawkeye can help.

With a unique option to Attach to any running .Net process, Hawkeye offers an impressive set of functionalities seen in no other product.

Features
Attach to any .Net Process.
Hawkeye can be injected in any .NET process allowing you to easily hook and modify other processes.
You can even hook into Visual Studio and modify some of its (.NET) properties (E.g.: the Properties Editor from VS).
Since version 1.1.9, Hawkeye has support for 64bit so you can now attach Hawkeye to any x86 or x64 process.
A properties editor like the VS editor that can be used to change the properties of any object or control at runtime.
Shows you all the properties that are defined on an object (even if they are not normally visible in the designer).
Shows you all the fields of an object organized by the class in the hierarchy that owns that property.
Shows all the methods of an object organized by the class and visibility of the method.
Provides a simple way to invoke methods on objects and pass arguments on any method (public, private ...).
Shows you all the events defined on an object and all the event listeners registered to listen to a specific event (e.g.: Form_Load).
You can even Invoke an event listener.
Shows process information including static information about Application, CurrentContext, CurrentThread, CurrentPrincipal, CurrentProcess, and garbage collection.
Supports back/forward navigation between the last edited objects, and supports navigation to child items in collections, enumerations or arrays (E.g.: the Controls collection of a Control).
Changes that you do to the code can be logged as C# code that can be just Copy&Pasted back into code.
How about "Show Source Code"?
You just started in a new project and you don't know where to start? Select your element, open Red Gate's .NET Reflector (formerly Lutz Roeder's .NET Reflector) and select Show source code. Hawkeye will immediately ask Reflector to show you the source code of the selected element being it a field, property, event, method or class.
Hawkeye was originally created by Corneliu I. Tusnea (his blog: http://www.acorns.com.au) from Readify (http://www.readify.net)
It is now maintained and supported by Olivier Dalet (http://odalet.wordpress.com)
Additional information on starting a new project is available here: Project Startup Guide.

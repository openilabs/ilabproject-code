Weblab Client README

Compilation environment:
Windows
Cygwin shell (www.cygwin.com) with make
Sun JDK >= 1.4

Ensure that javac, javadoc, jar, and make are on the executable path
Go to the client/java directory
Type make (this will give you a list of target options)
Choose an option and type e.g.
make graphical

The jar will be placed in the client/java/jar directory.

See client/java/test-graphical-applet.html for an example of
invocation syntax and parameters.

----

Ant utility is used with build.xml file contained in this codebase for compilations and packaging.  Please see ant documentation and the build.xml file for more information.

----

This version (client v7.1) moved from development branch to trunk and braches/client-v7_1 in the tree on March 29, 2012.
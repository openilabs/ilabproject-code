6/17/2003
WebLab Lab Server Controls Code Repository
James Hardison (hardison@alum.mit.edu)

The /controls directory in the lab server root is for code (organized into packages/namespaces/projects) that compile to class libraries used by the web application.  This includes the WebLabDataManagers namespace (Queue Manager, etc.) and the validation engine, for example.  

Note, lab server components that operated externally to the web site itself, such as the experiment execution engine should be maintained in their own directory in the lab server root.  

Compiled code from the .NET web app. controls may be placed in the /bin directory for proper integration with ASP.NET code.
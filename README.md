# Milou.Web.ClientResources

This is a library that makes it possible to add infinit caching for client resources like CSS and JavaScript files.

ASP.NET 4.6 with OWIN is the current target for this project.

## What it does

Milou.Web.ClientResources creates a hash sum based on all content in a directory, for example /content containing JavaScript and CSS files.

Normally there would be a reference in HTML like this to the static file custom.js

		<script type="application/javascript" src="/content/js/custom.js"></script>

This now becomes:

		<script type="application/javascript" src="/vstatic/964cf91456930706c5c81d80d0a97847/content/js/custom.js"></script>

The path is based on a virtual directory containing the hash sum. When the file is requested, caching headers are added to the response so the next requests won't hit the server.

## More examples

For more examples see Milou.Web.ClientResources.WebTests.Integration project.
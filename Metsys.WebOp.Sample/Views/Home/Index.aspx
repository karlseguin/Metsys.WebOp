<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">    
    <p>This is a sample website that shows how to use Metsys.WebOp within your own application. There are three main components:</p>
    <ul>
        <li><a href="#build">The Build Runner</a></li>
        <li><a href="#mvc">The MVC Framework</a></li>
        <li><a href="#module">The Modules</a></li>
    </ul>
    
    <h3>The Build Runner</h3><a name="build"></a>
    <p>The Build Runner is a console application which is meant to be run during your build-process that can be told to do a few things.</p>
    <p>The console application takes 1 argument - the root path of your asset folder. In this case, our root path is <code>&lt;SOMETHING&gt;/assets.</code></p>
    <p>For this project, we've set up a Pre-Build event in our project's properties. You can see it by right clicking on the project, picking <code>Properties</code> and then <code>Build Events</code>.You should see something like:</p>
    <%= Html.Image("build_events.png", 509, 368, "build event dialog") %>
    <p>We use VS.NET Macro's to make our paths relative to the solution and project file. This is what we have in there: <code>$(SolutionDir)..\BuildToolsForSample\WebOp.exe $(ProjectDir)assets\</code></p>
    <p>The Build Runner follows the command found in a file named <code>webop.dat</code> within the root of the supplied asset folder.</p>
    <p>Currently, 4 commands are supported: <code>combine</code>, <code>shrink</code>, <code>busting</code> and <code>zip</code></p>
    
    <h4>Combine</h4>
    <p>Combine is used to combine multiple files into a single file. Doing so reduces the number of requests your web server will have to deal with. Combine takes two arguments, the target file to combine to (relative to the asset path supplied to the console application), the second is a comma-delimited list of files to combine. Here are the two combine directives for this site:</p>
    <pre>
    combine: js\all.js: js\jquery.js, js\jquery.rollover.js, js\web.js
    combine: css\merged.css: css\reset.css, css\main.css
    </pre>
    <p>The first combines the three javascript files into a file called <code>js\all.js</code>, the second combines the two css files into <code>css\merged.css</code></p>
    
    <h4>Shrink</h4>
    <p>Shrink is used to minify javascript and css files. It takes 1 input, the array of files to shrink. Here's what we have in the sample project:</p>
    <pre>shrink: js\all.js, css\merged.css</pre>
    <p>Note that shrink will overwrite the original file - so you'll almost always want to do this on the output of a combine.</p>
    <p>Sometimes you'll want to shrink a file which isn't combined with any other. For example, <code>jquery.openid.js</code> might only be used on 1 page, and you rather not combine it with the <code>all.js</code> file. My recommendation would be to combine the file with none other, and shrink the output (this essentially turns combine into a copy operation):</p>
    <pre>
    combine: js\jquery.openid.min.js: js\jquery.openid.js
    shrink: js\jquery.openid.min.js
    </pre>
    <p><a href="http://www.codeplex.com/YUICompressor">YUI Compressor for .Net</a> is used for shrinking.</p>
    
    <h4>Busting</h4>
    <p>Busting generates a file which contains a hash for each file within our assets folder. This is used by the MVC Framework to append a hash to resources - allowing for long cache expiry that can be busted. The command takes 1 parameter which is the output file:</p>
    <pre>
    busting: hashes.dat
    </pre>
    <p>Busting also scoures your css files and looking for any images that may need to be busted - in other words, this'll change your css files.</p>
    
    <h4>Zip</h4>
    <p>Zip is used to pre-zip files. It takes 1 input, the array of files to zip. Here's what we have in the sample project:</p>
    <pre>zip: js\all.js, css\merged.css</pre>
    <p>This command is meant to be used by the zip http module</p>    
        
    <h3>The MVC Framework</h3><a name="mvc"></a>
    <p>The MVC Framework is really just a few extensions to the HtmlHelper class which let you generate tags for javascript, css and images. However, unlike normal tags, theses ones will take advantage of the combine and busting features of the runner. The most important thing is to configure WebOp in your application's start logic. Take a look at what this project does:</p>
    <pre>
    Mvc.WebOpConfiguration.Initialize
    (
        c =&gt; c.RootAssetPathIs(&quot;/assets/&quot;)
            .AssetHashesFilePathIs(Server.MapPath(&quot;~/assets/hashes.dat&quot;))
            .StylesAreIn(&quot;css&quot;)
    #if DEBUG
            .EnableSmartDebug(Server.MapPath(&quot;~/assets/webop.dat&quot;))
    #endif
    );
    </pre>
    <p>Everthing is pretty straight-forward. By default the framework assumes css files are stored in a folder name <code>styles</code> which is why we specifically overide it here (you can do the same for images and js files). We also tell it the path to our assets (this is something you'll always have to do), and since we want cache busting, we tell it the path to our generated hash files.</p>
    <p>The <code>SmartDebug</code> feature is simple - when set, it'll try to output the original files that make up a combine target. So, if we use:</p>
    <pre>&lt;%=Html.IncludeJs("all") %&gt;</pre>
    <p>with SmartDebug enabled, script tag for our three original files (which make up all.js) will be generated (web.js, jquery.js, and jquery.rollover.js).</p>
    <p>In addition to <code>IncludeJs</code>, you have <code>IncludeCss</code> (neither of which take an extension), <code>Image</code> and <code>ImageOver</code> (for use with the <code>jquery.rollover.js</code> plugin)</p>
    <p>It wouldn't be too hard to turn the MVC framework into a WebForms framework - if you are so inclined.</p>
    
    <h4>Image Over</h4>
    <p>The ImageOver method works with the <code>jquery.rollover.js</code> plugin and cache busting to make sure the over image is also cached and busted. In order for it to work, your hover image should be named <code>original_o.xxx</code>. This code:</p>
    <pre>&lt;%= Html.ImageOver(&quot;button.png&quot;, 100, 35, &quot;a button&quot;)%&gt;</pre>
    <p>Generates this image:</p>
    <%= Html.ImageOver("button.png", 100, 35, "a button")%>
    <p>Note that in our <code>web.js</code> file, we call <code>$('img.ro').rollover();</code></p>    
    
    <h3>The Modules</h3><a name="module"></a>  
    <p>There are two modules in the package - you can use none, either or both. The first removes useless headers and adds long-lasting cache headers to any js, css or image files. The other serves up pre-zipped js or css files when possible.</p>
    <p>The modules only works in IIS 7.0 integrated mode. While they aren't MVC specific, I figured it'd be better to put it here rather than having 2 separate assemblies.</p>
    <pre>
    &lt;system.webServer&gt;
        &lt;modules&gt;
            &lt;add name=&quot;WebOpHttpModule&quot; type=&quot;Metsys.WebOp.Mvc.WebOpHttpModule, Metsys.WebOp.Mvc&quot;/&gt;
            &lt;add name=&quot;WebOpZipHttpModule&quot; type=&quot;Metsys.WebOp.Mvc.WebOpZipHttpModule, Metsys.WebOp.Mvc&quot;/&gt;
        &lt;/modules&gt;
    &lt;/system.webServer&gt;    
    </pre>
</asp:Content>

To create your own custom views, for example to add your own css in the <head> section, choose which custom view you want to
override and copy and paste it into the Views/Custom folder.

These empty custom views are required so that the views in the hook points are found by asp.net core.

Views/Custom/*.cshtml files override the empty views in Views/Shared/EmptyCustomViews. Have a Utility/CustomViewLocationExpander.cs to see how this works.

There are various hook points for your own views. To see how the hook points are called have a look in :

 - Views/Shared/_Layout.cshtml
 - Views/Blog/ViewBlogPost.cshtml
 - Views/Blog/All.cshtml

 
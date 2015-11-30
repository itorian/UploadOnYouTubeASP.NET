<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Async="true" Inherits="EmptyWebForm._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>


    <script type="text/javascript" src="//code.jquery.com/jquery-2.1.4.min.js"></script>
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css" />
    <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js" type="text/javascript"></script>
    <script src="uploadOnYouTube.js"></script>


</head>
<body>
    <form id="form1" runat="server">
        <div>

            <input type="file" id="flyupdVideo" value=""/>
            <br />
            <br />
            <div id="progressBar" style="height: 20px; background-color: grey"></div>
            <br />
            <label id="statusMessage"></label>
            <br />
            <br />
            <input type="button" id="btnVedioSubmit" name="btnVedioSubmit" value="Upload" class="btn uploadVideo" />


        </div>
    </form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="dashboard.aspx.cs" Inherits="WebRole1.dashboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>PA4 Tim Ha</title>
    <meta charset="utf-8" />
    <link rel="stylesheet" href="style.css"/>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.1.1/jquery.min.js"></script>
    <script>
    $(document).ready(function () {
        GetTrieStats();
        GetStatus();
        GetStatsData();
        GetErrors();

        $("#loadCrawl").click(function () {
            $.ajax({
                type: 'POST',
                url: "admin.asmx/LoadCrawler",
                data: '{}',
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                }
            });
            
        });
        $("#startCrawl").click(function () {
            $.ajax({
                type: 'POST',
                url: "admin.asmx/StartCrawling",
                data: '{}',
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                }
            });
            
        });
        $("#stopCrawl").click(function () {
            $.ajax({
                type: 'POST',
                url: "admin.asmx/StopCrawling",
                data: '{}',
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                }
            });
        });
        $("#clearIndex").click(function () {
            $.ajax({
                type: 'POST',
                url: "admin.asmx/ClearIndex",
                data: '{}',
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                }
            });
        });

        $("#query").on("input", function () {
            var query = this.value;
            $.ajax({
                type: 'POST',
                url: "admin.asmx/GetPageTitle",
                data: '{"url":"' + query + '"}',
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    var obj = $.parseJSON(data.d);
                    var p = "<p><b>Page title: </b>" + obj + " </p>";
                    $("#results").html(p);
                }
            });
        });

    });
    function GetStatsData() {
        setInterval(function () {
            $.ajax({
                type: 'POST',
                url: "admin.asmx/GetStats",
                data: '{}',
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    var obj = $.parseJSON(data.d);
                    if (obj != null && obj.length > 0) {
                        var table = "<table>";
                        table = table + "<tr><th>CPU Usage (%) </th><td> " + obj[0] + " </td></tr>";
                        table = table + "<tr><th>Memory Usage (MB Free) </th><td> " + obj[1] + " </td></tr>";
                        table = table + "<tr><th># URLS crawled </th><td >" + obj[2] + " </td></tr>";
                        table = table + "<tr><th>Size of queue </th><td> " + obj[3] + " </td></tr>";
                        table = table + "<tr><th>Size of index </th><td> " + obj[4] + " </td></tr>";
                        table = table + "</table>";
                        $("#statsBoard").html(table);
                    }
                }
            });
        }, 250);
    }

    function GetStatus() {
        setInterval(function () {
            $.ajax({
                type: 'POST',
                url: "admin.asmx/CheckStatus",
                data: '{}',
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    var obj = $.parseJSON(data.d);
                    if (obj != null) {
                        var table = "<table>";
                        table = table + "<tr><th>Worker Status </th><td> " + obj + " </td></tr>";
                        table = table + "</table>";
                        $("#dashMsg").html(table);
                    }
                }
            });
        }, 250);
    }

    function GetErrors() {
        setInterval(function () {
            $.ajax({
                type: 'POST',
                url: "admin.asmx/GetErrors",
                data: '{}',
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    var obj = $.parseJSON(data.d);
                    if (obj != null) {
                        if (obj[0].length > 1) {
                            var table = "<table>";
                            table = table + "<tr><th>URL </th><td> " + obj[0] + " </td></tr>";
                            table = table + "<tr><th>Error message </th><td> " + obj[1] + " </td></tr>";
                            table = table + "</table>";
                            $("#errors").html(table);
                        }
                    }
                }
            });
        }, 1000);
    }

    function GetTrieStats() {
        $.ajax({
            type: 'POST',
            url: "admin.asmx/TrieStats",
            data: '{}',
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                var obj = $.parseJSON(data.d);
                var table = "<table>";
                console.log(obj);
                table = table + "<tr><th>Trie Count </th><td> " + obj[0] + " </td></tr>";
                table = table + "<tr><th>Last Title </th><td> " + obj[1] + " </td></tr>";
                table = table + "</table>";
                $("#trie").html(table);
            }
        });
        
    }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="navi">
            <input type="button" onclick="location.href='/search.aspx';" value="Home" />
            <input type="button" onclick="location.href='/dashboard.aspx';" value="Dashboard" />
            <input type="button" onclick="location.href='/admin.asmx';" value="Admin" />
        </div>
        <div>
            <button id="loadCrawl">Load Sitemaps</button>
            <button id="startCrawl">Start Crawling</button>
            <button id="stopCrawl">Stop Crawling</button>
            <button id="clearIndex">Stop & Clear Index</button>
        </div>
        <div id="trie">
        </div>
        <br /><br />
        <div id="dashMsg">
        </div>
        <br />
        <div id="dashboard">
            <div id="statsBoard">

            </div>
        </div>
        <br />

        <h3>Get Page title</h3>
        <div id="searchDiv">
            <b>Search url </b><input id="query" type="text" />
            <div id="results"></div>
        </div>
        <br />

        <h3>Most Recent Error</h3>
        <div id="errors">

        </div>
    </form>
</body>
</html>

﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="search.aspx.cs" Inherits="WebRole1.search" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>PA4 Tim Ha</title>
    <meta charset="utf-8" />
    <link rel="stylesheet" href="styleHome.css">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.1.1/jquery.min.js"></script>
    <script type="text/javascript" src="//cdn.chitika.net/getads.js" async></script>
    <script type="text/javascript">
        $(document).ready(function(){
            $("#queryBox").on("input", function () {
                var query = this.value.trim();
                //$.ajax({
                //    type: 'POST',
                //    url: "admin.asmx/SearchTrie",
                //    data: '{"prefix":"' + query + '"}',
                //    contentType: 'application/json; charset=utf-8',
                //    dataType: "json",
                //    success: function (data) {
                //        var obj = $.parseJSON(data.d);
                //        var table = "<table>";
                //        $.each(obj, function (index, value) {
                //            table = table + "<tr><td>" + value + "</td></tr>";
                //        });
                //        table = table + "</table>";
                //        $("#suggestions").html(table);
                //    }
                //});

                $.ajax({
                    type: 'POST',
                    url: "admin.asmx/GetUrls",
                    data: '{"query":"' + query + '"}',
                    contentType: 'application/json; charset=utf-8',
                    dataType: "json",
                    success: function (data) {
                        var obj = $.parseJSON(data.d);
                        var table = "<table>";
                        $.each(obj, function (index, value) {
                            var dateOnly = value.Item3.split(' ');
                            table = table + '<tr><th>' + value.Item1 + "</th></tr>";
                            table = table + '<tr><td class="tablUrl"><a href="' + value.Item2 + '">' + value.Item2 + "</a></td>";
                            table = table + '<td class="tableDate">' + dateOnly[0] + "</td></tr>";
                        });
                        table = table + "</table>";
                        $("#results").html(table);
                    }
                });

                $.ajax({
                    type: 'GET',
                    crossDomain: true,
                    url: "http://35.162.33.147/api.php",
                    data: {playerSearch: query},
                    contentType: 'application/json; charset=utf-8',
                    dataType: "jsonp",
                    success: function (data) {
                        var obj = data[0];
                        console.log(obj);
                        console.log(obj[4]);
                        console.log(obj["Ast"]);
                        console.log(obj["2"]);
                        //var table = "<table>";
                        //$.each(obj, function (index, value) {
                            //var dateOnly = value.Item3.split(' ');
                            //table = table + '<tr><th>' + value.Item1 + "</th></tr>";
                            //table = table + '<tr><td class="tablUrl"><a href="' + value.Item2 + '">' + value.Item2 + "</a></td>";
                            //table = table + '<td class="tableDate">' + dateOnly[0] + "</td></tr>";
                        //});
                        //table = table + "</table>";
                        //$("#results").html(table);
                    }
                });
            });
        });



        function PlaceAd() {
            if (window.CHITIKA === undefined) { window.CHITIKA = { 'units' : [] }; };
            var unit = {"calltype":"async[2]","publisher":"hjk9228","width":550,"height":250,"sid":"Chitika Default"};
            var placement_id = window.CHITIKA.units.length;
            window.CHITIKA.units.push(unit);
            var ad = '<div id="chitikaAdBlock-' + placement_id + '"></div>';
            document.write(ad);
        }
    </script>
    
</head>
<body>
        <div id="topbar">
            <div id="search">
                "Google" <input id="queryBox" type="text"/>
                <div id="suggestions"></div>
            </div>
            <div id="navi">
                <input type="button" onclick="location.href='/search.aspx';" value="Home" />
                <input type="button" onclick="location.href='/dashboard.aspx';" value="Dashboard" />
                <input type="button" onclick="location.href='/admin.asmx';" value="Admin" />
            </div>
        </div>
        <div id="mainContent">
            <div id="results">
            </div>
            <div id="adspace">
                <script>PlaceAd();</script>
            </div>
        </div>
</body>
</html>

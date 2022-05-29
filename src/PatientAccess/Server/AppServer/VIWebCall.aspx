<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VIWebCall.aspx.cs" Inherits="PatientAccess.VIWebCall" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset='utf-8' />
    <title>Scan Document</title>
    <link rel="shortcut icon" type="image/x-icon" href="Images/PA_icon.ico" />
    <meta http-equiv='X-UA-Compatible' content='IE=11' />
    <script src="jquery.min.js"></script>
    <script type='text/javascript'>

        var viwebSessionId = "";
        var viwebSessionIdfieldName = 'viwebSessionId';
        var pBAREmployeeID = "";

        function resizeWin() { window.resizeTo(225, 600); }

        function doScan(hspCd, patNumber, patName, Payor, currentDate, mrNumber, storedSessionId, pbarEmployeeID) {
            doClearInterval = false;
            if (storedSessionId) {
                viwebSessionId = storedSessionId;
            }
            if (pbarEmployeeID) {
                pBAREmployeeID = pbarEmployeeID;
            }
            var scanFormValues = {};
            scanFormValues['hspCode'] = hspCd;
            scanFormValues['patNumber'] = patNumber;
            scanFormValues['patName'] = patName;
            scanFormValues['Payor'] = Payor;
            scanFormValues['admitDate'] = currentDate;
            scanFormValues['mrNumber'] = mrNumber;
            if (viwebSessionId) {
                scanFormValues[viwebSessionIdfieldName] = viwebSessionId;
            }
            var iframewindow = document.getElementsByTagName('iframe')[0].contentWindow;
            iframewindow.postMessage(scanFormValues, '*');

        }

        function doView(hspCd, docID, storedSessionId, pbarEmployeeID) {
            doClearInterval = false;
            if (storedSessionId) {
                viwebSessionId = storedSessionId;
            }
            if (pbarEmployeeID) {
                pBAREmployeeID = pbarEmployeeID;
            }
            var arr = docID.split(',');
            var viewFormValues = {}
            viewFormValues['hspCode'] = hspCd;
            var fieldName = "docID";
            var fieldValue = [];
            for (var i = 0; i < arr.length; i++) {
                if (arr[i] !== '') {
                    fieldValue.push(arr[i]);
                }
            }
            viewFormValues[fieldName] = fieldValue
            if (viwebSessionId) {
                viewFormValues[viwebSessionIdfieldName] = viwebSessionId;
            }
            var iframewindow = document.getElementsByTagName('iframe')[0].contentWindow;
            iframewindow.postMessage(viewFormValues, '*');
        }

        function saveViwebSessionId(viwebSessionId) {
            $.ajax({
                type: "POST",
                url: "VIWebCall.aspx/SaveViwebSessionId",
                data: '{sessionId: "' + viwebSessionId + '",pbarEmployeeId: "' + pBAREmployeeID +'" }',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnSuccess,
                failure: function (response) {
                   console.log(response.d);
                }
                 });
        }
        function OnSuccess(response) {
            console.log(response.d);
        }
        function receivedVIWebSessionId(event) {
            if (event.data.startsWith('viwebSessionId:')) {
                let viwebSessionKey = event.data.split(":");
                viwebSessionId = viwebSessionKey[1];
                console.log("Test:" + viwebSessionId);
                if (viwebSessionId) {
                    saveViwebSessionId(viwebSessionId);
                }
            }
        }
        window.addEventListener("message", receivedVIWebSessionId);

    </script>
  
    <style type='text/css'>
        body, html {
            width: 100%;
            height: 100%;
            margin: 0;
            padding: 0;
        }

        .row-container {
            display: flex;
            width: 100%;
            min-height: 100vh;
            flex-direction: column;
        }

        .frame-class {
            flex-grow: 1;
            border: none;
            margin: 0;
            padding: 0;
        }

        .frame-div {
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
        }

        .frame-class1 {
            display: block;
            width: 100%;
            height: 100%;
            border: none;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
       <div class='frame-div'>
            <iframe name='vi-web-scan' runat="server" id="ifram_viweb_url"
                class='frame-class1'></iframe>
        </div>
    </form>
</body>
</html>

<%@ Page Language="C#" 
         AutoEventWireup="false" 
         EnableViewStateMac="false" 
         CodeBehind="Default.aspx.cs"
         Inherits="PatientAccess.AppServer.Default" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>Patient Access for Windows © :: Web Installer</title>
    <style>
        DIV
        {
            font-size: 11px;
            color: #000000;
            font-family: Verdana, Arial, Helvetica, sans-serif;
        }
        TD
        {
            font-size: 11px;
            color: #000000;
            font-family: Verdana, Arial, Helvetica, sans-serif;
        }
        A
        {
            font-size: 11px;
            color: #000000;
            font-family: Verdana, Arial, Helvetica, sans-serif;
        }
        A:visited
        {
            font-size: 11px;
            color: #000000;
            font-family: Verdana, Arial, Helvetica, sans-serif;
        }
        H2
        {
            font-size: 20px;
            font-family: Verdana, Arial, Helvetica, sans-serif;
        }
        .buttons
        {
            font-size: 11px;
            width: 90px;
            font-family: Verdana, Arial, Helvetica, sans-serif;
        }
        .mainTitle
        {
            font-weight: 900;
            font-size: 14px;
            font-family: Verdana, Arial, Helvetica, sans-serif;
        }
        .thisframe
        {
            font-size: 11px;
            font-family: Verdana, Arial, Helvetica, sans-serif;
        }
        .bold
        {
            font-weight: 900;
        }
        .dataentry
        {
            width: 150px;
        }
        TABLE.err
        {
            border-right: #6798c7 1px solid;
            border-top: #6798c7 1px solid;
            border-left: #6798c7 1px solid;
            border-bottom: #6798c7 1px solid;
        }
        TH.err
        {
            font-size: 14px;
            color: #ffffff;
            font-family: Verdana, Arial, Helvetica, sans-serif;
            background-color: #6798c7;
        }
        TD.err
        {
            font-size: 10px;
            color: #ff0000;
            font-family: Verdana, Arial, Helvetica, sans-serif;
        }
        UL.downloadlink
        {
            margin-left: 15px;
        }
    </style>
</head>
<body>
    <form runat="server">
    <table height="100%" width="100%" align="center">
        <tbody>
            <tr>
                <td valign="middle" align="center">
                    <table style="border-right: #6798c7 1px solid; border-top: #6798c7 1px solid; border-left: #6798c7 1px solid;
                        border-bottom: #6798c7 1px solid" height="440" cellspacing="0" cellpadding="0"
                        width="600" border="0">
                        <tbody>
                            <tr>
                                <td valign="top" width="594" background="images/headerbg.gif" height="98">
                                    <img height="98" alt="Patient Access Installation Wizard" src="images/topHeader.png"
                                        width="496">
                                </td>
                            </tr>
                            <tr>
                                <td valign="top">
                                    <div style="padding-right: 10px; padding-left: 10px; padding-bottom: 10px; padding-top: 30px">
                                        <!-- Welcome Panel -->
                                        <asp:Panel ID="Welcome" runat="server" Visible="false">
                                            <div class="mainTitle">
                                                Welcome to the Patient Access remote installation wizard.</div>
                                            <div>
                                                <br>
                                                <br>
                                                <strong>Hardware Requirements:</strong></div>
                                            <div style="padding-left: 10px; padding-bottom: 1px; padding-top: 1px">
                                                <ul>
                                                    <li>Memory: 256 MB minimum, 512+ recommended
                                                        <li>Processor Speed: 600 MHz minimum, 1 GHz or more recommended
                                                            <li>1024px x 768px screen resolution and 32 bit graphics (Normal size DPI settings/96
                                                                DPI)
                                                                <li>32 Megabytes free hard drive space + 4 Mb per Windows account that will use Patient
                                                                    Access </li>
                                                </ul>
                                            </div>
                                            <div>
                                                <br>
                                                <strong>Software Requirements:</strong></div>
                                            <div style="padding-left: 10px; padding-top: 1px">
                                                <ul>
                                                    <li>Microsoft Windows 2000 Professional or Windows XP Professional
                                                        <li>Microsoft .NET Framework Version 1.1 w/Service Pack 1 (1.1.4322.2032)<br>
                                                            <ul class="downloadlink">
                                                                <li><a title="Download .NET Framework 1.1" href="http://www.microsoft.com/downloads/details.aspx?FamilyId=262D25E3-F589-4842-8157-034D1E7CF3A3&amp;displaylang=en"
                                                                    target="_self">Download .NET Framework 1.1</a>
                                                                    <li><a title="Download .NET Framework 1.1 Service Pack 1" href="http://www.microsoft.com/downloads/details.aspx?familyid=A8F5654F-088E-40B2-BBDB-A83353618B38&amp;displaylang=en"
                                                                        target="_self">Download .NET Framework 1.1 Service Pack 1</a> </li>
                                                            </ul>
                                                            <li>Microsoft .NET Framework Version 2.0 (2.0.50727.42)<br>
                                                                <ul class="downloadlink">
                                                                    <li><a title="Download .NET Framework 2.0" href="http://www.microsoft.com/downloads/details.aspx?FamilyID=0856eacb-4362-4b0d-8edd-aab15c5e04f5&displaylang=en"
                                                                        target="_self">Download .NET Framework 2.0</a> </li>
                                                                </ul>
                                                                <li>Microsoft .NET Framework Version 2.0 (2.0.50727.42)<br>
                                                                    <ul class="downloadlink">
                                                                        <li><a title="Download .NET Framework 2.0" href="http://www.microsoft.com/downloads/details.aspx?FamilyID=0856eacb-4362-4b0d-8edd-aab15c5e04f5&displaylang=en"
                                                                            target="_self">Download .NET Framework 2.0</a> </li>
                                                                    </ul>
                                                                    <li>Background Intelligent Transfer Service (<a title="Determining the Version of BITS on a Computer"
                                                                        href="http://windowssdk.msdn.microsoft.com/en-us/library/aa362837.aspx" target="_self">BITS</a>)<br>
                                                                        <ul class="downloadlink">
                                                                            <li><a title="Download BITS Version 1.5 for Windows 2000" href="http://www.microsoft.com/downloads/details.aspx?FamilyID=8c6ef6c8-2abf-43c7-ab51-e0c53303086d&amp;DisplayLang=en"
                                                                                target="_self">Download BITS Version 1.5 for Windows 2000</a>
                                                                                <li><a title="Download BITS Version 2.0 for Windows XP and 2003" href="http://www.microsoft.com/downloads/details.aspx?FamilyID=B93356B1-BA43-480F-983D-EB19368F9047&amp;displaylang=en"
                                                                                    target="_self">Download BITS Version 2.0 for Windows XP and 2003</a> </li>
                                                                        </ul>
                                                                        <li>Windows Installer 2.0 or above (<a title="Download Windows Installer" href="http://www.microsoft.com/downloads/results.aspx?freetext=&amp;productID=9DD7A078-F8B0-457F-96CD-39C8428C2AEE&amp;categoryId=&amp;period=&amp;sortCriteria=popularity&amp;nr=20&amp;DisplayLang=en"
                                                                            target="_self">Download</a>)
                                                                            <li>Adobe Acrobat Reader (<a title="Download Adobe Acrobat Reader" href="http://www.adobe.com/products/acrobat/readstep2.html"
                                                                                target="_self">Download</a>)
                                                                                <li>Microsoft Hotfixes (Install in the following sequence)
                                                                                    <ol>
                                                                                        <li>Microsoft Hotfix-226395 (<a title="Download Microsoft Hotfix-226395" href="Downloads/MicrosoftHotfixes/226395_EN/NDP1.1sp1-KB893005-X86.exe">Download</a>)&nbsp;(<a
                                                                                            title="Microsoft Hotfix-226395 Readme" href="Downloads/MicrosoftHotfixes/226395_EN/Readme.txt">Readme</a>)
                                                                                            <li>Microsoft Hotfix-237428 (<a title="Download Microsoft Hotfix-237428" href="Downloads/MicrosoftHotfixes/237428_EN/NDP1.1sp1-KB899511-X86.exe">Download</a>)&nbsp;(<a
                                                                                                title="Microsoft Hotfix-237428 Readme" href="Downloads/MicrosoftHotfixes/237428_EN/Readme.txt">Readme</a>)
                                                                                            </li>
                                                                                    </ol>
                                                                                </li>
                                                </ul>
                                            </div>
                                        </asp:Panel>
                                        <!-- License Panel -->
                                        <asp:Panel ID="License" runat="server" Visible="false">
                                            <table cellspacing="0" cellpadding="0" border="0">
                                                <tbody>
                                                    <tr>
                                                        <td>
                                                            <span class="mainTitle">Patient Access License</span>
                                                        </td>
                                                        <td nowrap align="right">
                                                        </td>
                                                        <tr>
                                                            <td colspan="2">
                                                                &nbsp;
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td valign="top" align="right" colspan="2">
                                                            </td>
                                                        </tr>
                                                </tbody>
                                            </table>
                                            <br>
                                            <br>
                                            <div style="padding-right: 20px" align="left">
                                                <aspcheckbox id="chkIAgree" runat="server" checked="false" text=" I Agree" />
                                                <aspcustomvalidator id="req_pa_acknowledge_license" runat="server" errormessage="You must accept the license agreement to proceed."
                                                    onservervalidate="CheckLicenseAgreement" />
                                            </div>
                                        </asp:Panel>
                                        <!-- SelectApp Panel -->
                                        <asp:Panel ID="SelectApp" runat="server" Visible="false">
                                            <span class="mainTitle">Select Application</span>
                                            <br>
                                            <br>
                                            <div>
                                                Choose the Patient Access application component you would like to to install.</div>
                                            <div style="padding-left: 20px; padding-top: 20px">
                                                <div style="padding-left: 20px; padding-top: 10px">
                                                    Available Components:
                                                    <asp:DropDownList ID="ddlAppList" runat="server">
                                                    </asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="reqAppName" runat="server" ErrorMessage="*" Enabled="true"
                                                        ControlToValidate="ddlAppList"></asp:RequiredFieldValidator></div>
                                            </div>
                                        </asp:Panel>
                                        <!-- GatherInfo Panel -->
                                        <asp:Panel ID="GatherInfo" runat="server" Visible="false">
                                            <span class="mainTitle">Download Patient Access</span>
                                            <br>
                                            <br>
                                            <div>
                                                Enter the following information to download the Patient Access client.</div>
                                            <div style="padding-left: 20px; padding-top: 20px">
                                                <table>
                                                    <tr>
                                                        <td align="left">
                                                            First name:
                                                        </td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtFirstName" runat="server" CssClass="dataentry"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="req_pa_fname" runat="server" Enabled="true" ControlToValidate="txtFirstName">* First name is required!</asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                            Last name:
                                                        </td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtLastName" runat="server" CssClass="dataentry" TextMode="Password"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="req_pa_lname" runat="server" Enabled="true" ControlToValidate="txtLastName">* Last name is required!</asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                            Phone number:
                                                        </td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtPhone" runat="server" CssClass="dataentry"></asp:TextBox>example:
                                                            (555)555-1212
                                                            <asp:RequiredFieldValidator ID="req_pa_phone" runat="server" Enabled="true" ControlToValidate="txtPhone">* Phone number is required!</asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                            E-mail:
                                                        </td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtEmail" runat="server" CssClass="dataentry" TextMode="Password"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="req_pa_email" runat="server" Enabled="true" ControlToValidate="txtEmail">* E-mail addressis required!</asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                            Confirm e-mail:
                                                        </td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtConfirmEmail" runat="server" CssClass="dataentry" TextMode="Password"></asp:TextBox>
                                                            <asp:CompareValidator ID="comp_pa_email" runat="server" ControlToValidate="txtConfirmEmail"
                                                                CssClass="color:red" ControlToCompare="txtEmail" Enabled="True" EnableClientScript="True">* E-mail addresses do not match!</asp:CompareValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2">
                                                            &nbsp;
                                                        </td>
                                                    </tr>
                                                </table>
                                                <p>
                                                </p>
                                            </div>
                                        </asp:Panel>
                                        <!-- Download Panel -->
                                        <asp:Panel ID="Download" runat="server" Visible="false">
                                            <span class="mainTitle">Download Patient Access for Windows © Installer</span>
                                            <div>
                                                Select the Download button to begin downloading
                                                <asp:Literal ID="ltrVersion" runat="server"></asp:Literal></div>
                                        </asp:Panel>
                                        <!-- Done Panel -->
                                        <asp:Panel ID="Done" runat="server" Visible="false">
                                            <span class="mainTitle">Download Complete!</span>
                                        </asp:Panel>
                                        <!-- Errors Panel -->
                                        <asp:Panel ID="Errors" runat="server" Visible="false">
                                            <span class="mainTitle">An Error Has Occurred</span>
                                            <br>
                                            <br>
                                            <div>
                                                <asp:Literal ID="ltrErrorMsg" runat="server" /></div>
                                            <div style="padding-left: 20px; color: red; padding-top: 10px">
                                                <asp:Literal ID="lblErrMsg" runat="server" EnableViewState="False"></asp:Literal>
                                            </div>
                                            <asp:Repeater ID="lstMessages" runat="server">
                                                <HeaderTemplate>
                                                    <table class="err" width="580px" border="1" cellpadding="0" cellspacing="0">
                                                        <tr>
                                                            <th class="err" width="100px">
                                                                Module
                                                            </th>
                                                            <th class="err">
                                                                Message
                                                            </th>
                                                        </tr>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <tr>
                                                        <td class="err">
                                                            <%# ((InstallerMessage)Container.DataItem).Module %>
                                                        </td>
                                                        <td class="err">
                                                            <%# ((InstallerMessage)Container.DataItem).Message %>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    </table>
                                                </FooterTemplate>
                                            </asp:Repeater>
                                        </asp:Panel>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td valign="bottom">
                                    <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                        <tr>
                                            <td>
                                                <img src="images/blcorner.gif">
                                            </td>
                                            <td align="right">
                                                <img src="images/brCorner.gif">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" bgcolor="#6798c7" height="45">
                                    <div style="padding-right: 30px">
                                        <asp:Button ID="Previous" runat="server" CssClass="buttons" Text="< Previous"></asp:Button>&nbsp;
                                        <asp:Button ID="Next" runat="server" CssClass="buttons" Text="Next >"></asp:Button>
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
        </tbody>
    </table>
    </form>
</body>
</html>

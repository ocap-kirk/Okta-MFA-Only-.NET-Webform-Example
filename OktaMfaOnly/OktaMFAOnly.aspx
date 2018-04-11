<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OktaMFAOnly.aspx.cs" Inherits="OktaMfaOnly.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>


    <script src="https://ok1static.oktacdn.com/assets/js/sdk/okta-signin-widget/2.7.0/js/okta-sign-in.min.js" type="text/javascript"></script>
	<link href="https://ok1static.oktacdn.com/assets/js/sdk/okta-signin-widget/2.7.0/css/okta-sign-in.min.css" type="text/css" rel="stylesheet" />
	<link href="https://ok1static.oktacdn.com/assets/js/sdk/okta-signin-widget/2.7.0/css/okta-theme.css" type="text/css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox ID="username" runat="server" />
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
          ControlToValidate="username"
          ErrorMessage="Username is a required field."
          ForeColor="Red">
        </asp:RequiredFieldValidator>

    
        <asp:Button ID="Button1" runat="server" Text="Submit Username" OnClick="Button1_Click1" />
        <asp:TextBox ID="sessionToken" runat="server"  style="display:none;" />

        <asp:Button ID="Button2" runat="server" Text="Submit sessionToken" OnClick="Button2_Click1" style="display:none;" />

        <asp:TextBox ID="ResultOfMFA" runat="server" style="display:none;" />

        
    </div>
    </form>
    
    <br />
	<div id="okta-login-container"></div>
	<div id="message"></div>
	<script type="text/javascript">
	    var serStateToken ="<%=stateToken%>";
	  var oktaSignIn = new OktaSignIn({
	    baseUrl: "https://tkirk.oktapreview.com",
	    stateToken: serStateToken   
	  });
	  
	  if (serStateToken != "") {
	      // No session, show the login form
	      oktaSignIn.renderEl(
          { el: '#okta-login-container' },
          function success(res) {
              // Nothing to do in this case, the widget will automatically redirect
              // the user to Okta for authentication, then back to this page if successful
              console.log(res.session.token);
              document.getElementById("<%=sessionToken.ClientID%>").value = res.session.token;
              document.getElementById("<%=Button2.ClientID%>").click();
          },
          function error(err) {
              // handle errors as needed
              console.error(err);
          }
          );
	  }
	  
	</script>
</body>
</html>

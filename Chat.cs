@{
    ViewBag.Title = "Chat";
}
</div>

<div id="chatBackground">

    <div id="chatHeader">
        <h3>Connect</h3>
        Chat with: &nbsp&nbsp&nbsp&nbsp <input type="checkbox" id="all" name="all" value="all" checked="checked"/>All &nbsp&nbsp&nbsp&nbsp or &nbsp <input type="text" id="target" name="target" size="33" placeholder="User Name"/>
    </div>

    <div id="chatInput">
        <textarea name="messageInput" id="messageInput" rows="6" cols="80"></textarea>
    </div>

    <div id="chatButtons">
        <button id="sendMessage">Send</button>
        <span style="position:absolute; right:0px;"> @Html.ActionLink("Exit Chat", "Index", "Home")</span>
    </div>

    <div id="chatConversation">
        <ul id="conversation"></ul>
    </div>

    <div id="chatUserList">
        <p id="userListHeader">Who's On?</p>
        <ul id="userList"></ul>
    </div>
</div>

<div class="container body-content">

    @section scripts {
        <script src="/Scripts/jquery.signalR-2.2.0.js"></script>
        <script src="/signalr/hubs"></script>
    }
    <script>
        //select user from the list of current users, set as addressee
        function SelectUser(user) {
                userName = user.innerText;
                $('#target').val(userName);
                $('#all').prop("checked", false);
                $('#messageInput').focus();
        };

        $(function () {

            var senderName = "@ViewData["senderName"]";
            var chat = $.connection.ChatHub;

            // start the connection, then...
            $.connection.hub.start().done(function () {
                $('#conversation').append('<li>Chat connection established.</li>');
                //register this user with the hub
                chat.server.registerConxnId(senderName);
                //set the function of the Send button, depending on whether -chat with All- is checked
                $('#sendMessage').click(function () {
                    if ($('input[name="all"]').is(':checked')) {
                        chat.server.sendMessage(senderName, $('#messageInput').val());
                    }
                    else {
                        chat.server.sendMessage(senderName, $('#messageInput').val(), $('input[name="target"]').val());
                    }
                    // Clear text box and reset focus for next comment.
                    $('#messageInput').val('').focus();
                });
                // Ready to chat: Set initial focus to message input box.
                $('#messageInput').focus();
            });

            //Display messages received from the hub
            chat.client.displayMessage = function(senderName, message) {
                $('#conversation').append('<li><strong>' + senderName + '</strong>: ' + message + '</li>');
            };

            //Display list of current users received from the hub
            chat.client.displayListOfConnected = function(connectedUsers) {
                $('#userList').empty();
                var counter = 0;
                for (user in connectedUsers) {
                    $('#userList').append('<li><a onclick="SelectUser(this)">' + connectedUsers[counter] + '</a></li>');
                    counter += 1;
                }
            };
        });
    </script>
(async function () {
    const userName = document.getElementById("user").value;
    const userMessage = document.getElementById("userMessage");
    const btnSend = document.getElementById("btnSend");
    const userMessages = document.getElementById("userMessages");

    $(btnSend).click(async () => {
        const message = $(userMessage).val();

        if (!message || message === '') {
            return;
        }

        try {
            await connection.invoke("SendMessage", {
                message, userName
            });

            $(userMessage).val('');
        } catch (err) {
            console.error(err);
        }

    });

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chat")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    async function start() {
        try {
            await connection.start();
            console.log(`SignalR Connected - user: ${userName}.`);
        } catch (err) {
            console.log(err);
            setTimeout(start, 5000);
        }
    };

    connection.onclose(async () => {
        await start();
    });

    connection.on("ReceiveMessage", (payload) => {
        const {userName, message, formattedCreatedOn} = payload;
        const li = document.createElement("li");
        li.innerHTML = `<strong>[${formattedCreatedOn}], ${userName}:</strong>${message}`;
        document.getElementById("userMessages").appendChild(li);
    });

// Start the connection.
    start();
})();



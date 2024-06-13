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
            location.reload();
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
        const { userName, message, formattedCreatedOn } = payload;
        const div = document.createElement("li");
        div.innerHTML = `
         <div class="d-flex flex-row p-3">
            <img src="https://img.icons8.com/color/48/000000/circled-user-male-skin-type-7.png" width="30" height="30">
            <div class="flex flex-col ml-2">
                <div><strong>${userName}</strong></div>
                <div class="bg-white p-1">
                    <div class="mt-1 ml-3">${message}</div>
                </div>
                <div class="text-muted mt-1" style="font-size: 11px;">${formattedCreatedOn}</div>
            </div>
         </div>
        `;
        document.getElementById("userMessages").appendChild(div);
        location.reload();
    });

    // Start the connection.
    start();
})();

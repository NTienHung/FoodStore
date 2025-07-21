document.addEventListener("DOMContentLoaded", () => {
    const userInput = document.getElementById("userInput");
    const sendBtn = document.getElementById("sendBtn");
    const chatDisplay = document.getElementById("chatDisplay");

    let conversationHistory = [
        { role: "system", content: "You are a helpful assistant." }
    ];

    const displayMessage = (sender, message) => {
        const safeSender = sender.replace(/\s+/g, '-'); // Chuyển khoảng trắng thành "-"
        const messageDiv = document.createElement("div");
        messageDiv.classList.add("message", safeSender);
        messageDiv.innerHTML = `<p><strong>${sender}:</strong> ${message.replace(/\n/g, '<br>')}</p>`;
        chatDisplay.appendChild(messageDiv);
        chatDisplay.scrollTop = chatDisplay.scrollHeight;
    };

    const fetchAIResponse = async (userMessage) => {
        displayMessage("Food Store", "Loading...");

        conversationHistory.push({ role: "user", content: userMessage });

        try {
            const response = await fetch("http://localhost:7031/api/ChatGPTAPI", {
                method: "POST",
                headers: {
                    "accept": "*/*",
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    message: userMessage
                })
            });

            if (!response.ok) throw new Error("Error fetching response from server");

            const text = await response.text();
            let data;
            try {
                data = JSON.parse(text);
            } catch (e) {
                displayMessage("Food Store", text);
                return;
            }

            const aiMessage = data.choices[0].message.content.trim();
            chatDisplay.lastChild.remove();
            displayMessage("Food Store", aiMessage);
            conversationHistory.push({ role: "assistant", content: aiMessage });

        } catch (error) {
            displayMessage("Error", error.message);
        }
    };

    sendBtn.addEventListener("click", () => {
        const userMessage = userInput.value.trim();
        if (userMessage) {
            displayMessage("You", userMessage);
            userInput.value = "";
            fetchAIResponse(userMessage);
        }
    });

    userInput.addEventListener("keypress", (event) => {
        if (event.key === "Enter") sendBtn.click();
    });
});

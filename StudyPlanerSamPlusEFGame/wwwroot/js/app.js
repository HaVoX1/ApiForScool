const listEl = document.querySelector("#subjects");
const messageEl = document.querySelector("#message");
const createForm = document.querySelector("#create-form");
const createName = document.querySelector("#create-name");

function showMessage(text) {
    messageEl.hidden = !text;
    messageEl.textContent = text ?? "";
}

async function readError(response) {
    if (response.status === 404) {
        return "Предмет не найден";
    }

    try {
        const body = await response.json();
        if (body?.message) {
            return body.message;
        }
    } catch {  }

    return "Не удалось выполнить запрос";
}

async function loadSubjects() {
    const response = await fetch("/api/subjects");
    if (!response.ok) {
        showMessage(await readError(response));
        return;
    }
    showMessage("");
    const subjects = await response.json();
    listEl.replaceChildren(...subjects.map(renderSubject));
}

function renderSubject(subject) {
    const item = document.createElement("li");
    item.className = "item";

    const title = document.createElement("span");
    title.className = "item-name";
    title.textContent = subject.name;

    const rename = document.createElement("form");
    rename.className = "rename";
    const input = document.createElement("input");
    input.type = "text";
    input.maxLength = 100;
    input.required = true;
    input.value = subject.name;
    const save = document.createElement("button");
    save.type = "submit";
    save.textContent = "Сохранить";
    rename.append(input, save);
    rename.addEventListener("submit", async (event) => {
        event.preventDefault();
        const response = await fetch(`/api/subjects/${subject.id}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ name: input.value.trim() })
        });
        if (!response.ok) {
            showMessage(await readError(response));
            return;
        }
        await loadSubjects();
    });

    const remove = document.createElement("button");
    remove.type = "button";
    remove.className = "danger";
    remove.textContent = "Удалить";
    remove.addEventListener("click", async () => {
        const response = await fetch(`/api/subjects/${subject.id}`, {
            method: "DELETE"
        });
        if (!response.ok) {
            showMessage(await readError(response));
            return;
        }
        await loadSubjects();
    });

    item.append(title, rename, remove);
    return item;
}

createForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    const response = await fetch("/api/subjects", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ name: createName.value.trim() })
    });
    if (!response.ok) {
        showMessage(await readError(response));
        return;
    }
    createName.value = "";
    await loadSubjects();
});

loadSubjects();

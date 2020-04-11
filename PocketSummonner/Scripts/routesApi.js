async function SumByName(name) {
    return await $.get("../Api/SumByName", {
        name
    })
}
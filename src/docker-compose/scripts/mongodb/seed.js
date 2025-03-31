db = db.getSiblingDB('FinancialDB');

db.Users.insertMany([
    {
        _id: UUID("40365e90-7821-4710-8743-9d4f80a6b442"),
        name: "API Empresa 1 User",
        userName: "e1",
        roles: ["credito"],
        apiKey: "f2ab89f2-c65e-4eb5-8e2f-02e65e3ab313"
    },
    {
        _id: UUID("699970c5-f183-4436-8958-2bc22b9b9a7d"),
        name: "API Empresa 1 User",
        userName: "e2",
        roles: ["credito", "debito"],
        apiKey: "9e3f2b78-d79e-4dc1-9b19-13a96d109af6"
    }
]);

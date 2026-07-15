-- Id: 1
-- Description: Создание таблиц

CREATE TABLE IF NOT EXISTS Users (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL
);

CREATE TABLE IF NOT EXISTS Roles (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL
);

-- Связка "Пользователь - Роль" (многие ко многим)
CREATE TABLE IF NOT EXISTS UserRoles (
    user_id INT NOT NULL REFERENCES Users(id) ON DELETE CASCADE,
    role_id INT NOT NULL REFERENCES Roles(id) ON DELETE CASCADE,
    PRIMARY KEY (user_id, role_id)
);

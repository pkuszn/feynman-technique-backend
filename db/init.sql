CREATE DATABASE IF NOT EXISTS `ft-db`;

CREATE TABLE IF NOT EXISTS `ft-db`.`part_of_speech` (
	id int NOT NULL PRIMARY KEY,
	name VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS `ft-db`.`word` (
	id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
	word VARCHAR(50) NOT NULL,
	part_of_speech int NOT NULL,
	created_date DATETIME NOT NULL,
	context varchar(100),
	link varchar(200),
	FOREIGN KEY (part_of_speech)
        REFERENCES part_of_speech(id)
        ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS `ft-db`.`role` (
	id int NOT NULL PRIMARY KEY,
	name VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS `ft-db`.`user` (
	id int NOT NULL AUTO_INCREMENT PRIMARY KEY,
	role int NOT NULL,
	name VARCHAR(50) NOT NULL,
	password VARCHAR(50) NOT NULL,
	created_date DATETIME NOT NULL,
	FOREIGN KEY (role)
        REFERENCES part_of_speech(id)
        ON DELETE CASCADE
);

INSERT INTO `ft-db`.`part_of_speech`(id, name)
VALUES
	(0, 'none'),
	(1, 'rzeczownik'),
	(2, 'przymiotnik'),
	(3, 'liczebnik'),
	(4, 'przyslowek'),
	(5, 'czasownik'),
	(6, 'zaimek'),
	(7, 'przyimek'),
	(8, 'spójnik'),
	(9, 'punkt'),
	(10, 'wykrzyknik'),
	(11, 'partykuła'),
	(12, 'zaimek_wskazujący'),
	(13, 'czasownik_pomocniczy'),
	(14, 'rzeczownik_odpowiedni'),
	(15, 'spójnik koordynacyjny'),
	(16, 'symbol');

INSERT INTO `ft-db`.`role`(id, name)
VALUES
    (0, "admin"),
    (1, "user");

INSERT INTO `ft-db`.`user`(id, role, name, password, created_date)
VALUES
	(0, 0, "root", "rootFT", CURRENT_TIMESTAMP);
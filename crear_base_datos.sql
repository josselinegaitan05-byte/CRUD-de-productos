CREATE DATABASE IF NOT EXISTS productosdb CHARACTER SET utf8mb4;
USE productosdb;

DROP TABLE IF EXISTS `productos`;
CREATE TABLE IF NOT EXISTS `productos` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `precio` decimal(10,2) NOT NULL,
  `cantidad` int NOT NULL,
  `imagen` longblob NOT NULL,
  PRIMARY KEY (`id`)
);

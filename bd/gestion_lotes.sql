-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 25-06-2026 a las 02:19:37
-- Versión del servidor: 10.4.27-MariaDB
-- Versión de PHP: 8.2.0

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `gestion_lotes`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `forma_pagos`
--

CREATE TABLE `forma_pagos` (
  `id_forma_pago` int(11) NOT NULL,
  `id_recibo_persona_fisica` int(11) DEFAULT NULL,
  `id_recibo_persona_juridica` int(11) DEFAULT NULL,
  `destinatario` varchar(255) NOT NULL,
  `efectivo` decimal(19,4) DEFAULT NULL,
  `transferencia` decimal(19,4) DEFAULT NULL,
  `dolar_monto` decimal(19,4) DEFAULT NULL,
  `dolar_cotizacion` decimal(19,4) DEFAULT NULL,
  `euro_monto` decimal(19,4) DEFAULT NULL,
  `euro_cotizacion` decimal(19,4) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `forma_pagos`
--

INSERT INTO `forma_pagos` (`id_forma_pago`, `id_recibo_persona_fisica`, `id_recibo_persona_juridica`, `destinatario`, `efectivo`, `transferencia`, `dolar_monto`, `dolar_cotizacion`, `euro_monto`, `euro_cotizacion`) VALUES
(100, 25, NULL, 'Gobierno', '24000.0000', '0.0000', '0.0000', '0.0000', '0.0000', '0.0000'),
(101, 25, NULL, 'Cmcpsl', '16000.0000', '0.0000', '0.0000', '0.0000', '0.0000', '0.0000'),
(102, 25, NULL, 'Dpip', '480.0000', '0.0000', '0.0000', '0.0000', '0.0000', '0.0000');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `lotes`
--

CREATE TABLE `lotes` (
  `id_lote` int(11) NOT NULL,
  `n_lote` int(11) NOT NULL,
  `marca` varchar(255) NOT NULL,
  `modelo` varchar(255) NOT NULL,
  `dominio` varchar(255) NOT NULL,
  `anio` varchar(255) NOT NULL,
  `base` decimal(19,4) NOT NULL,
  `creado_por` varchar(255) NOT NULL,
  `fecha_creacion` date NOT NULL DEFAULT current_timestamp(),
  `estado` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `lotes`
--

INSERT INTO `lotes` (`id_lote`, `n_lote`, `marca`, `modelo`, `dominio`, `anio`, `base`, `creado_por`, `fecha_creacion`, `estado`) VALUES
(3, 2, 'T-Cross', 'T-Cross', 'T-Cross', '2019', '20000.0000', 'admin@admin', '2025-12-05', 1),
(4, 3, 'Toyota Corolla Cross', 'Toyota Corolla Cross', 'Toyota Corolla Cross', '2021', '2900.0000', 'admin@admin', '2025-12-06', 1),
(5, 4, 'Peugeot 208', ' Peugeot 208', 'Peugeot 208', '2019', '2900.0000', 'admin@admin', '2025-12-06', 1),
(6, 5, 'Fiat Cronos', 'Fiat Cronos', 'Fiat Cronos', '2018', '3000.0000', 'admin@admin', '2025-12-06', 1),
(7, 6, 'Toyota Hilux', 'Toyota Hilux', 'Toyota Hilux', '2005', '9000.0000', 'admin@admin', '2025-12-06', 1),
(8, 7, 'Volkswagen Amarok', 'Volkswagen Amarok', 'Volkswagen Amarok', '2019', '5000.0000', 'admin@admin', '2025-12-06', 1),
(9, 8, 'Ford Ranger', 'Ford Ranger', 'Ford Ranger', '2008', '900.0000', 'admin@admin', '2025-12-06', 1),
(10, 9, 'Renault Alaskan', 'Renault Alaskan', 'Renault Alaskan', '2009', '1800.0000', 'admin@admin', '2025-12-06', 1),
(11, 1, 'Ford', '2012', 'FR', '2012', '10000.0000', 'admin@admin', '2026-01-21', 1),
(12, 10, 'Ford Ranger', '2020', '2020', '2020', '10000.0000', 'admin@admin', '2026-02-09', 1),
(14, 11, 'Volkswagen', 'Polo', 'AB123CD', '2010', '80000.0000', 'admin@admin', '2026-06-24', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `parametros`
--

CREATE TABLE `parametros` (
  `id_parametros` int(11) NOT NULL,
  `honorarios` decimal(6,4) NOT NULL,
  `sellado` decimal(6,4) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `parametros`
--

INSERT INTO `parametros` (`id_parametros`, `honorarios`, `sellado`) VALUES
(1, '20.0000', '0.6000');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `recibo_persona_fisica`
--

CREATE TABLE `recibo_persona_fisica` (
  `id_recibo_persona_fisica` int(11) NOT NULL,
  `id_lote` int(11) DEFAULT NULL,
  `nombre` varchar(255) NOT NULL,
  `apellido` varchar(255) NOT NULL,
  `tipo_dni` varchar(255) NOT NULL,
  `dni` varchar(255) NOT NULL,
  `telefono` varchar(255) NOT NULL,
  `codigo_postal` varchar(255) NOT NULL,
  `email` varchar(255) NOT NULL,
  `creado_por` varchar(255) NOT NULL,
  `domicilio` varchar(255) NOT NULL,
  `fecha_creacion` date NOT NULL DEFAULT current_timestamp(),
  `provincia` varchar(255) NOT NULL,
  `estado` tinyint(1) NOT NULL DEFAULT 1,
  `pago_lote` decimal(10,0) NOT NULL DEFAULT 30,
  `precio_subastado` decimal(19,4) NOT NULL,
  `honorarios` decimal(6,4) NOT NULL,
  `sellado` decimal(6,4) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `recibo_persona_fisica`
--

INSERT INTO `recibo_persona_fisica` (`id_recibo_persona_fisica`, `id_lote`, `nombre`, `apellido`, `tipo_dni`, `dni`, `telefono`, `codigo_postal`, `email`, `creado_por`, `domicilio`, `fecha_creacion`, `provincia`, `estado`, `pago_lote`, `precio_subastado`, `honorarios`, `sellado`) VALUES
(25, 14, 'Jose', 'Pedro', 'Dni', '4342341112', '2668129012', 'D5070', 'Jose@gmail.com', 'admin@admin', 'La Punta', '2026-06-24', 'San Luis', 1, '30', '80000.0000', '20.0000', '0.6000');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `recibo_persona_juridica`
--

CREATE TABLE `recibo_persona_juridica` (
  `id_recibo_persona_juridica` int(11) NOT NULL,
  `id_lote` int(11) NOT NULL,
  `razon_social` varchar(255) NOT NULL,
  `apoderado_socio` varchar(255) NOT NULL,
  `tipo` varchar(255) NOT NULL,
  `numero` varchar(255) NOT NULL,
  `telefono` varchar(255) NOT NULL,
  `codigo_postal` varchar(255) NOT NULL,
  `email` varchar(255) NOT NULL,
  `creado_por` varchar(255) NOT NULL,
  `domicilio` varchar(255) NOT NULL,
  `fecha_creacion` date NOT NULL DEFAULT current_timestamp(),
  `provincia` varchar(255) NOT NULL,
  `pago_lote` decimal(10,0) NOT NULL DEFAULT 30,
  `precio_subastado` decimal(19,4) NOT NULL,
  `estado` tinyint(1) NOT NULL DEFAULT 1,
  `honorarios` decimal(6,4) NOT NULL,
  `sellado` decimal(6,4) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuarios`
--

CREATE TABLE `usuarios` (
  `id_usuario` int(11) NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `apellido` varchar(255) NOT NULL,
  `rol` int(11) NOT NULL,
  `fecha_creacion` date NOT NULL DEFAULT current_timestamp(),
  `clave` varchar(255) NOT NULL,
  `avatarUrl` varchar(255) DEFAULT NULL,
  `email` varchar(255) NOT NULL,
  `estado` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuarios`
--

INSERT INTO `usuarios` (`id_usuario`, `nombre`, `apellido`, `rol`, `fecha_creacion`, `clave`, `avatarUrl`, `email`, `estado`) VALUES
(18, 'admin', 'admin', 1, '2025-11-06', 'PJaQVsoZLo39CH/BQi6SoJ2mu8gD6tQfGAvyButelqg=', '/images/avatars/9de16f48-47f7-45a5-9d8d-3fe1a0a19bb5.png', 'admin@admin', 0),
(19, 'Test', 'test', 2, '2025-11-06', 'Hh5PJIf2spPKILQESYS5hbDBI9MT0YpCqENeX/PwcPE=', '/images/avatars/default-avatar.png', 'test@test', 0),
(21, 'Pedro ', 'Gomez', 2, '2026-02-24', 'Hh5PJIf2spPKILQESYS5hbDBI9MT0YpCqENeX/PwcPE=', '/images/avatars/default-avatar.png', 'Pedro@pedro', 0);

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `forma_pagos`
--
ALTER TABLE `forma_pagos`
  ADD PRIMARY KEY (`id_forma_pago`),
  ADD KEY `fk_forma_pagos_recibo_persona_fisica` (`id_recibo_persona_fisica`),
  ADD KEY `fk_id_forma_pago_id_recibos` (`id_recibo_persona_juridica`);

--
-- Indices de la tabla `lotes`
--
ALTER TABLE `lotes`
  ADD PRIMARY KEY (`id_lote`);

--
-- Indices de la tabla `parametros`
--
ALTER TABLE `parametros`
  ADD PRIMARY KEY (`id_parametros`);

--
-- Indices de la tabla `recibo_persona_fisica`
--
ALTER TABLE `recibo_persona_fisica`
  ADD PRIMARY KEY (`id_recibo_persona_fisica`),
  ADD KEY `fk_id_lotes_id_recibos` (`id_lote`);

--
-- Indices de la tabla `recibo_persona_juridica`
--
ALTER TABLE `recibo_persona_juridica`
  ADD PRIMARY KEY (`id_recibo_persona_juridica`),
  ADD KEY `fk_id_lotes_id_recibos2` (`id_lote`);

--
-- Indices de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`id_usuario`),
  ADD UNIQUE KEY `email` (`email`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `forma_pagos`
--
ALTER TABLE `forma_pagos`
  MODIFY `id_forma_pago` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=103;

--
-- AUTO_INCREMENT de la tabla `lotes`
--
ALTER TABLE `lotes`
  MODIFY `id_lote` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=15;

--
-- AUTO_INCREMENT de la tabla `parametros`
--
ALTER TABLE `parametros`
  MODIFY `id_parametros` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT de la tabla `recibo_persona_fisica`
--
ALTER TABLE `recibo_persona_fisica`
  MODIFY `id_recibo_persona_fisica` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=26;

--
-- AUTO_INCREMENT de la tabla `recibo_persona_juridica`
--
ALTER TABLE `recibo_persona_juridica`
  MODIFY `id_recibo_persona_juridica` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `id_usuario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=22;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `forma_pagos`
--
ALTER TABLE `forma_pagos`
  ADD CONSTRAINT `fk_id_forma_pago_id_recibos` FOREIGN KEY (`id_recibo_persona_juridica`) REFERENCES `recibo_persona_juridica` (`id_recibo_persona_juridica`) ON UPDATE CASCADE;

--
-- Filtros para la tabla `recibo_persona_fisica`
--
ALTER TABLE `recibo_persona_fisica`
  ADD CONSTRAINT `fk_id_lotes_id_recibos` FOREIGN KEY (`id_lote`) REFERENCES `lotes` (`id_lote`) ON UPDATE CASCADE;

--
-- Filtros para la tabla `recibo_persona_juridica`
--
ALTER TABLE `recibo_persona_juridica`
  ADD CONSTRAINT `fk_id_lotes_id_recibos2` FOREIGN KEY (`id_lote`) REFERENCES `lotes` (`id_lote`) ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

-- phpMyAdmin SQL Dump
-- version 3.2.5
-- http://www.phpmyadmin.net
--
-- Serveur: localhost
-- Généré le : Sam 19 Novembre 2011 à 21:41
-- Version du serveur: 5.1.41
-- Version de PHP: 5.3.0

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Base de données: `homiweb`
--
CREATE DATABASE `homiweb` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci;
USE `homiweb`;

-- --------------------------------------------------------

--
-- Structure de la table `config`
--

CREATE TABLE IF NOT EXISTS `config` (
  `config_id` smallint(6) unsigned NOT NULL AUTO_INCREMENT,
  `config_nom` text NOT NULL,
  `config_valeur` text NOT NULL,
  `config_description` text NOT NULL,
  PRIMARY KEY (`config_id`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=9 ;

--
-- Contenu de la table `config`
--

INSERT INTO `config` (`config_id`, `config_nom`, `config_valeur`, `config_description`) VALUES
(1, 'meteo_codeville', 'FRXX1222', 'Code Ville Prevision weather.com (FRXX0076)'),
(2, 'meteo_icone', '2', 'Numero du pack d icones meteo'),
(3, 'meteo_codevillereleve', 'LUXX0003', 'Code Ville Releve weather.com (FRXX0076)'),
(4, 'serveur_ip', '192.168.1.2', 'Adresse IP du serveur'),
(5, 'serveur_port', '3852', 'Port du serveur (3852)'),
(6, 'menu_seticone', '1', 'Numero du set d icones pour le menu (wwwimagesmenux)'),
(8, 'logs_nbparpage', '1000', 'Nombre logs à afficher par page');

-- --------------------------------------------------------

--
-- Structure de la table `devices`
--

CREATE TABLE IF NOT EXISTS `devices` (
  `devices_id` int(11) NOT NULL AUTO_INCREMENT,
  `devices_nom` text NOT NULL,
  `devices_ssid` text NOT NULL,
  `devices_driver` text NOT NULL,
  `devices_datemaj` datetime NOT NULL,
  `devices_valeur` text NOT NULL,
  `type` text NOT NULL,
  PRIMARY KEY (`devices_id`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=2 ;

--
-- Contenu de la table `devices`
--

INSERT INTO `devices` (`devices_id`, `devices_nom`, `devices_ssid`, `devices_driver`, `devices_datemaj`, `devices_valeur`, `type`) VALUES
(1, 'Température Salon', 'sse541gseg6s7e1f6s16s7ef', 'RFXreceiver', '2011-09-17 00:00:00', '15', 'temp');

-- --------------------------------------------------------

--
-- Structure de la table `devices_zones`
--

CREATE TABLE IF NOT EXISTS `devices_zones` (
  `devices_zones_id` int(11) NOT NULL AUTO_INCREMENT,
  `devices_zones_devicesid` int(11) NOT NULL,
  `devices_zones_zonesid` int(11) NOT NULL,
  `devices_zones_coordx` int(11) NOT NULL,
  `devices_zones_coordy` int(11) NOT NULL,
  `devices_zones_visible` tinyint(1) NOT NULL,
  PRIMARY KEY (`devices_zones_id`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=2 ;

--
-- Contenu de la table `devices_zones`
--

INSERT INTO `devices_zones` (`devices_zones_id`, `devices_zones_devicesid`, `devices_zones_zonesid`, `devices_zones_coordx`, `devices_zones_coordy`, `devices_zones_visible`) VALUES
(1, 1, 1, 50, 50, 1);

-- --------------------------------------------------------

--
-- Structure de la table `drivers`
--

CREATE TABLE IF NOT EXISTS `drivers` (
  `drivers_id` int(11) NOT NULL AUTO_INCREMENT,
  `drivers_nom` text NOT NULL,
  `drivers_actif` tinyint(1) NOT NULL,
  PRIMARY KEY (`drivers_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

--
-- Contenu de la table `drivers`
--


-- --------------------------------------------------------

--
-- Structure de la table `logs`
--

CREATE TABLE IF NOT EXISTS `logs` (
  `logs_id` bigint(20) NOT NULL AUTO_INCREMENT,
  `logs_source` tinytext NOT NULL,
  `logs_description` text NOT NULL,
  `logs_date` datetime NOT NULL,
  PRIMARY KEY (`logs_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

--
-- Contenu de la table `logs`
--


-- --------------------------------------------------------

--
-- Structure de la table `plan`
--

CREATE TABLE IF NOT EXISTS `plan` (
  `plan_id` smallint(6) unsigned NOT NULL AUTO_INCREMENT,
  `plan_composant` smallint(6) unsigned NOT NULL,
  `plan_nomplan` text NOT NULL,
  `plan_top` smallint(6) NOT NULL,
  `plan_left` smallint(6) NOT NULL,
  `plan_visible` tinyint(1) NOT NULL,
  PRIMARY KEY (`plan_id`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=4 ;

--
-- Contenu de la table `plan`
--

INSERT INTO `plan` (`plan_id`, `plan_composant`, `plan_nomplan`, `plan_top`, `plan_left`, `plan_visible`) VALUES
(1, 33, 'rdc_temp', 320, 430, 1),
(2, 22, 'rdc_temp', 225, 235, 1),
(3, 23, 'rdc_temp', 165, 450, 1);

-- --------------------------------------------------------

--
-- Structure de la table `users`
--

CREATE TABLE IF NOT EXISTS `users` (
  `id` smallint(6) unsigned NOT NULL AUTO_INCREMENT,
  `login` tinytext NOT NULL,
  `pwd` tinytext NOT NULL,
  `droits` smallint(6) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=3 ;

--
-- Contenu de la table `users`
--

INSERT INTO `users` (`id`, `login`, `pwd`, `droits`) VALUES
(1, 'administrateur', 'homiweb', 9),
(2, 'visiteur', 'homiweb', 1);

-- --------------------------------------------------------

--
-- Structure de la table `webcams`
--

CREATE TABLE IF NOT EXISTS `webcams` (
  `webcams_id` smallint(6) unsigned NOT NULL AUTO_INCREMENT,
  `webcams_nom` tinytext NOT NULL,
  `webcams_description` text NOT NULL,
  `webcams_lien` text NOT NULL,
  `webcams_accueil` tinyint(4) NOT NULL,
  PRIMARY KEY (`webcams_id`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=20 ;

--
-- Contenu de la table `webcams`
--

INSERT INTO `webcams` (`webcams_id`, `webcams_nom`, `webcams_description`, `webcams_lien`, `webcams_accueil`) VALUES
(1, 'Metz', 'Centre Pompidou', 'http://www.metz.fr/pompidou_webcam/image.jpg', 0),
(2, 'Dudelange', 'Weather', 'http://www.weather.webroom.net/webcam/cam.jpg', 1),
(3, 'Luxembourg', 'Pont Adolphe', 'http://www.mtp.public.lu/webcam/current.jpg?1285587633805', 0),
(6, 'La Bresse', 'Station', 'http://www.trinum.com/ibox/labresse/Images/labresse2.jpg', 0),
(5, 'Les orres', 'station', 'http://www.lesorres.com/images/stories/webcam/netcam.jpg', 0),
(18, 'Maison', 'Cam1', 'http://192.168.1.5/snapshot.cgi?user=domos&amp;amp;pwd=domos&amp;amp;next_url=', 0),
(7, 'A3', 'France - Croix de Bettembourg', 'http://www.cita.lu/info_trafic/cameras/images/cccam_66.jpg', 0),
(8, 'A3', 'France - Croix de Bettembourg 2', 'http://www.cita.lu/info_trafic/cameras/images/cccam_63.jpg', 0),
(9, 'A3', 'Bettembourg - Croix de Bettembourg', 'http://www.cita.lu/info_trafic/cameras/images/cccam_59.jpg', 0),
(10, 'A3', 'Aire de Berchem - Bettembourg', 'http://www.cita.lu/info_trafic/cameras/images/cccam_53.jpg', 0),
(11, 'A3', 'Croix de Gasperich - Aire de Berchem', 'http://www.cita.lu/info_trafic/cameras/images/cccam_55.jpg', 0),
(12, 'A3', 'Aire de Berchem - Croix de Gasperich', 'http://www.cita.lu/info_trafic/cameras/images/cccam_51.jpg', 0),
(13, 'A3', 'Croix de Gasperich - Aire de Berchem', 'http://www.cita.lu/info_trafic/cameras/images/cccam_41.jpg', 0),
(14, 'A3', 'Croix de Gasperich - Luxembourg', 'http://www.cita.lu/info_trafic/cameras/images/cccam_47.jpg', 0),
(15, 'A13', 'Croix de Bettembourg - Dudelange', 'http://www.cita.lu/info_trafic/cameras/images/cccam_167.jpg', 0),
(16, 'A13', 'Hellange - Croix de Bettembourg', 'http://images.newmedia.lu/rtl.lu/auto/traffik/traffikcams/cccam_200.jpg', 0),
(4, 'La Bresse', 'Station', 'http://www.trinum.com/ibox/labresse/Images/cam1_partimont_00001.jpg', 0),
(19, 'Maison', 'Cam1 Video', 'http://192.168.1.5/videostream.cgi?user=domos&amp;amp;pwd=domos&amp;amp;next_url=', 0);

-- --------------------------------------------------------

--
-- Structure de la table `zones`
--

CREATE TABLE IF NOT EXISTS `zones` (
  `zones_id` int(11) NOT NULL AUTO_INCREMENT,
  `zones_nom` text NOT NULL,
  `zones_icone` text NOT NULL,
  `zones_plan` text NOT NULL,
  `zones_ordre` smallint(6) NOT NULL,
  PRIMARY KEY (`zones_id`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=7 ;

--
-- Contenu de la table `zones`
--

INSERT INTO `zones` (`zones_id`, `zones_nom`, `zones_icone`, `zones_plan`, `zones_ordre`) VALUES
(1, 'rdc', 'rdc.png', 'rdc.jpg', 2),
(2, 'Premier étage', 'unn.png', 'unn.jpg', 3),
(3, 'cave', 'cav.png', 'cav.jpg', 1),
(4, 'deuxième étage', 'deu.png', 'deu.jpg', 4),
(5, 'Extérieur coté rue', 'ex1.png', 'ex1.jpg', 5),
(6, 'Extérieur coté terrasse', 'ex2.png', 'ex2.jpg', 6);

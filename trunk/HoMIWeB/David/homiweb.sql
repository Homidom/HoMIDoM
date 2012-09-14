-- phpMyAdmin SQL Dump
-- version 3.5.1
-- http://www.phpmyadmin.net
--
-- Client: localhost
-- Généré le: Ven 14 Septembre 2012 à 07:01
-- Version du serveur: 5.5.24-log
-- Version de PHP: 5.4.3

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Base de données: `homiweb`
--

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
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` text NOT NULL,
  `ssid` text NOT NULL,
  `driver` text NOT NULL,
  `datemaj` datetime NOT NULL,
  `value` text NOT NULL,
  `type` text NOT NULL,
  `adresse1` text NOT NULL,
  `adresse2` text NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=78 ;

--
-- Contenu de la table `devices`
--

INSERT INTO `devices` (`id`, `name`, `ssid`, `driver`, `datemaj`, `value`, `type`, `adresse1`, `adresse2`) VALUES
(2, 'Cave : HumiditÃ©', '816bf683-6e28-4805-b1f6-b032117d4b0d', '', '0000-00-00 00:00:00', '0', 'HUMIDITE', '27138', ''),
(3, 'Cave : TempÃ©rature', '9a0f0c4d-4587-4bb6-8992-48179a6e85ea', '', '0000-00-00 00:00:00', '0', 'TEMPERATURE', '27138', ''),
(4, 'Cave technique : compteur A', '2b520027-5de6-499e-bbca-907981c1b210', '', '0000-00-00 00:00:00', '0', 'COMPTEUR', '670000000B22881D', 'A'),
(5, 'Cave technique : compteur B', '769815cb-600e-4245-9916-653157c35672', '', '0000-00-00 00:00:00', '0', 'COMPTEUR', '670000000B22881D', 'B'),
(6, 'Cave technique : contact', '61d3807b-bf7c-43eb-a699-8f3e192a9260', '', '0000-00-00 00:00:00', '', 'CONTACT', '6C00000045798B12', ''),
(7, 'Cave technique : TempÃ©rature', 'c74fc883-f5b8-4425-a9a6-16100ad2f6df', '', '0000-00-00 00:00:00', '0', 'TEMPERATURE', '9218', ''),
(8, 'Cave technique : TempÃ©rature 2', '46c1ab09-4bec-41ae-932e-2b39e338d88a', '', '0000-00-00 00:00:00', '0', 'TEMPERATURE', '3C0008015A808B10', ''),
(9, 'Cave vin : HumiditÃ©', 'b1e4809c-fe5c-4c62-b103-8a4cf696c601', '', '0000-00-00 00:00:00', '0', 'HUMIDITE', '39682', ''),
(10, 'Cave vin : TempÃ©rature', 'bd26f35e-f0aa-4237-bb10-3eaece52e320', '', '0000-00-00 00:00:00', '0', 'TEMPERATURE', '39682', ''),
(11, 'Chambre : HP', 'c878cebe-1400-4139-b6ec-de3bbe7e7124', '', '0000-00-00 00:00:00', '', 'AUDIO', 'C:foobar2000chambre_1_1foobar2000.exe', ''),
(12, 'Chambre : TempÃ©rature', '170e28d3-19ca-455c-b942-e397aecdfa93', '', '0000-00-00 00:00:00', '0', 'TEMPERATURE', '50436', ''),
(13, 'Chambre : Volet', '24b7677a-b4e2-405b-800f-26f09552d090', '', '0000-00-00 00:00:00', '0', 'VOLET', 'P5', ''),
(14, 'Chambre ami : HumiditÃ©', '521d8316-852b-4f03-ab4d-22f530bf93af', '', '0000-00-00 00:00:00', '0', 'HUMIDITE', '3329', ''),
(15, 'Chambre ami : TempÃ©rature', 'fe244d5c-28ee-4660-86b8-0d0eadac816e', '', '0000-00-00 00:00:00', '0', 'TEMPERATURE', '3329', ''),
(16, 'Chambre bÃ©bÃ© : HumiditÃ©', '2e94636e-832f-4e7f-802c-92ca12d76eb9', '', '0000-00-00 00:00:00', '0', 'HUMIDITE', '11522', ''),
(17, 'Chambre bÃ©bÃ© : TempÃ©rature', '75d5863c-0315-4df7-a70c-e107d0a16182', '', '0000-00-00 00:00:00', '0', 'TEMPERATURE', '11522', ''),
(18, 'Chambre bÃ©bÃ© : Volet', 'f430caf3-105e-435b-a6cf-352c400e673f', '', '0000-00-00 00:00:00', '0', 'VOLET', 'P6', ''),
(19, 'Compteur gaz virtuel', 'c26a0575-b859-40bd-9c3f-ef60d0a6d668', '', '0000-00-00 00:00:00', '0', 'COMPTEUR', 'gaz', ''),
(20, 'Couloir 1er : detecteur mvt 1', 'dbb440d8-d5c4-44a8-96e1-2cb8af68502c', '', '0000-00-00 00:00:00', '', 'DETECTEUR', 'D1', ''),
(21, 'Couloir 1er : detecteur mvt 2', '16befea1-f5d4-4215-998c-591f9b387cab', '', '0000-00-00 00:00:00', '', 'DETECTEUR', 'D9', ''),
(22, 'Couloir 1er : detecteur obscuritÃ©', 'ad7fc233-9f21-49b5-9da3-f4a06e485dc2', '', '0000-00-00 00:00:00', '', 'DETECTEUR', 'D2', ''),
(23, 'Couloir 1er : Lampe escalier', 'e8e8a771-e20a-4379-963c-f8b380348d91', '', '0000-00-00 00:00:00', '100', 'LAMPE', 'L2', ''),
(24, 'Couloir 1er : tempÃ©rature', 'c69269c2-c316-41a7-a7e8-938b734ed47c', '', '0000-00-00 00:00:00', '0', 'TEMPERATURE', '45058', ''),
(25, 'Couloir 2eme : detecteur mvt', '15c118f4-1d57-4d29-bb0c-fe6aaa555b5b', '', '0000-00-00 00:00:00', '', 'DETECTEUR', 'D3', ''),
(26, 'Couloir 2eme : detecteur obscuritÃ©', 'fbb66bf4-8574-4294-b485-856d64190d70', '', '0000-00-00 00:00:00', '', 'DETECTEUR', 'D4', ''),
(27, 'Couloir 2eme : HumiditÃ©', 'aec92f46-9c69-4dbc-aa5f-f9eac0b00029', '', '0000-00-00 00:00:00', '0', 'HUMIDITE', '62978', ''),
(28, 'Couloir 2eme : TempÃ©rature', '212a4c59-42ee-4210-bba1-450f9f9fb3cd', '', '0000-00-00 00:00:00', '0', 'TEMPERATURE', '62978', ''),
(29, 'Couloir RDC : Lampe IkÃ©a', 'c410b497-7a61-42ef-9c54-15f0e9612b25', '', '0000-00-00 00:00:00', '0', 'LAMPE', 'M12', ''),
(30, 'Couloir RDC : TempÃ©rature', 'bb803486-24b3-4520-ada0-a1c64e68cb22', '', '0000-00-00 00:00:00', '0', 'TEMPERATURE', '12546', ''),
(31, 'Couloirs : Spots Jour', 'cad8b364-6b41-45d6-833c-78c1affa1b07', '', '0000-00-00 00:00:00', '0', 'LAMPE', 'L11', ''),
(32, 'Couloirs : Spots Nuit', '99a27dde-78a2-4d3c-af04-25be154b12ea', '', '0000-00-00 00:00:00', '100', 'LAMPE', 'L10', ''),
(33, 'Cuisine : Lustre', 'f5b09e4f-6fef-4eb7-83ea-a04cd80d7f28', '', '0000-00-00 00:00:00', '0', 'LAMPE', 'L6', ''),
(34, 'Cuisine : Lustre tÃ©lÃ©commande', '9a18c42d-839d-47e6-a4d6-22ad35d13fb2', '', '0000-00-00 00:00:00', '', 'SWITCH', 'M5', ''),
(35, 'Cuisine : TempÃ©rature', '079fac5a-58da-45a5-b173-756a2157fa18', '', '0000-00-00 00:00:00', '0', 'TEMPERATURE', '5890', ''),
(36, 'Cuisine : Volet', 'f63549b7-e7a6-4ad5-be45-a26163c37fc8', '', '0000-00-00 00:00:00', '0', 'VOLET', 'P3', ''),
(37, 'Cuisine : Volet tÃ©lÃ©commande', '7727e541-8585-40a7-a317-b4d72f35b907', '', '0000-00-00 00:00:00', '', 'SWITCH', 'M11', ''),
(38, 'FastPooling Lampes', 'e23b57ce-3ca7-44d1-be3a-f68700048bcc', '', '0000-00-00 00:00:00', '0', 'LAMPE', 'L', ''),
(39, 'Freebox', '96b77400-b60e-48cd-af78-97e2eba610d1', '', '0000-00-00 00:00:00', '', 'FREEBOX', 'http://hd1.freebox.fr', '97489781'),
(40, 'HOMI_Jour', 'soleil01', '', '0000-00-00 00:00:00', '1', 'GENERIQUEBOOLEEN', 'N/A', ''),
(41, 'Jour detecteur', '39ceb32c-3b8b-4920-b9a7-c3e3b1525f8e', '', '0000-00-00 00:00:00', '', 'DETECTEUR', '01EFF32-10', ''),
(42, 'Meteo Algrange', '3ebcd1d4-f991-4e4f-9afa-d321dbbd9865', '', '0000-00-00 00:00:00', '', 'METEO', 'algrange', ''),
(43, 'RFXPower', '13f6c4df-39c7-4d84-a4af-218ff5f67f3c', '', '0000-00-00 00:00:00', '33.92', 'COMPTEUR', '02F2-754', ''),
(44, 'Rue : HumiditÃ©', 'd2ec2641-9557-4e22-a709-b1571113b32f', '', '0000-00-00 00:00:00', '0', 'HUMIDITE', '38404', ''),
(45, 'Rue : TempÃ©rature', 'aef24a30-9b19-47d6-aea6-c9d07be96763', '', '0000-00-00 00:00:00', '0', 'TEMPERATURE', '38404', ''),
(46, 'Salle de jeu : TempÃ©rature', '4ba39810-026d-407a-a61c-e23a59d9e9fe', '', '0000-00-00 00:00:00', '0', 'TEMPERATURE', '44802', ''),
(47, 'Salon : Etoiles tÃ©lÃ©commande', '5c255422-e37d-454b-827f-b8b42929cf9f', '', '0000-00-00 00:00:00', '0', 'LAMPE', 'M16', ''),
(48, 'Salon : Lustre', '58bcae2b-40e1-47d5-a6f9-56d13aeba0af', '', '0000-00-00 00:00:00', '100', 'LAMPE', 'L4', ''),
(49, 'Salon : Lustre tÃ©lÃ©commande', '55074929-0b75-49e6-a0d9-2449f43b0298', '', '0000-00-00 00:00:00', '1', 'SWITCH', 'M3', ''),
(50, 'Salon : Petite lampe', '25ef965c-a576-4c24-983d-5bdf32e5d851', '', '0000-00-00 00:00:00', '0', 'LAMPE', 'M2', ''),
(51, 'Salon : Spot Ambiance canap-tv', '7f13aac8-5a80-42e1-9dd8-587200c0d693', '', '0000-00-00 00:00:00', '0', 'LAMPE', 'M1', ''),
(52, 'Salon : Spots', '6ba8c17c-9b19-48e1-b920-74d5595b2c91', '', '0000-00-00 00:00:00', '0', 'LAMPE', 'L7', ''),
(53, 'Salon : Spots tÃ©lÃ©commande', 'ef990524-3b29-4759-a9a8-ced91939b037', '', '0000-00-00 00:00:00', '1', 'SWITCH', 'M4', ''),
(54, 'Salon : Telco Chacon 1', '7dc774bc-c89b-4ff5-b924-da2a1385da2c', '', '0000-00-00 00:00:00', '', 'SWITCH', '02F4416-1', ''),
(55, 'Salon : TempÃ©rature', '658136a7-ba1d-4d64-a97f-157fb0ce9c05', '', '0000-00-00 00:00:00', '0', 'TEMPERATURE', '37636', ''),
(56, 'Salon : Volet', '7aa5d0c0-70b6-4efd-8919-65b8144647da', '', '0000-00-00 00:00:00', '0', 'VOLET', 'P1', ''),
(57, 'Salon : Volet tÃ©lÃ©commande', '0decb43f-1706-42e8-8cd4-61747c6ed551', '', '0000-00-00 00:00:00', '', 'SWITCH', 'M9', ''),
(58, 'SAM : HumiditÃ©', 'f60d0dca-9ad5-4beb-9e5b-723128c5a155', '', '0000-00-00 00:00:00', '0', 'HUMIDITE', '62210', ''),
(59, 'SAM : Lustre', '38c74b4e-f88f-4a21-8e96-a605b438ad29', '', '0000-00-00 00:00:00', '0', 'LAMPE', 'L5', ''),
(60, 'SAM : Lustre tÃ©lÃ©commande', '7323ef43-55b3-4cd9-81fc-4d36d1aa4c2f', '', '0000-00-00 00:00:00', '', 'SWITCH', 'M6', ''),
(61, 'SAM : Spots ext', '71a78217-b2ca-4bfd-b31f-178780de3d10', '', '0000-00-00 00:00:00', '0', 'LAMPE', 'L3', ''),
(62, 'SAM : Spots ext tÃ©lÃ©commande', '55fe03cd-a75a-4077-aab9-ac78d642b705', '', '0000-00-00 00:00:00', '', 'SWITCH', 'M8', ''),
(63, 'SAM : Spots int', 'b0d31145-5903-4002-a9e2-bd2f7249a75b', '', '0000-00-00 00:00:00', '0', 'LAMPE', 'L8', ''),
(64, 'SAM : Spots int tÃ©lÃ©commande', '66c9c2b1-2083-436a-82a4-92f3a7b23616', '', '0000-00-00 00:00:00', '0', 'LAMPE', 'M7', ''),
(65, 'SAM : TempÃ©rature', '8d1a6b10-5626-4fd1-9740-736db8cef369', '', '0000-00-00 00:00:00', '0', 'TEMPERATURE', '62210', ''),
(66, 'SAM : Volet', '80189ec8-c8bf-40cd-9916-d4ea33612cae', '', '0000-00-00 00:00:00', '0', 'VOLET', 'P2', ''),
(67, 'SAM : Volet tÃ©lÃ©commande', 'c03f2e6f-8948-4bbc-926a-f26886cad14b', '', '0000-00-00 00:00:00', '', 'SWITCH', 'M10', ''),
(68, 'SAM-Cuisine : Grosse Etoiles', 'f1811553-6105-44d7-a790-9761f3eb8cbc', '', '0000-00-00 00:00:00', '0', 'LAMPE', 'M15', ''),
(69, 'SDB : HP', 'be027754-8890-4911-a2a2-2ce159492d31', '', '0000-00-00 00:00:00', '', 'AUDIO', 'sdb', ''),
(70, 'SDB : HumiditÃ©', 'ee31da85-4ed2-447d-9e51-addb466bc356', '', '0000-00-00 00:00:00', '0', 'HUMIDITE', '58114', ''),
(71, 'SDB : TempÃ©rature', 'ec7e45db-217b-46d3-8f3a-df483018b84e', '', '0000-00-00 00:00:00', '0', 'TEMPERATURE', '58114', ''),
(72, 'SDB : Volet', '2b316d76-b8a4-4031-a1de-e4782f8c096e', '', '0000-00-00 00:00:00', '0', 'VOLET', 'P4', ''),
(73, 'Terasse : Detecteur  LuminositÃ©', '8f5ef8bb-8f49-4fd1-9a65-c1d6abaddf87', '', '0000-00-00 00:00:00', '', 'DETECTEUR', 'D8', ''),
(74, 'Terasse : detecteur mvt', 'a3242f17-92be-4420-b756-ac11ebd8438a', '', '0000-00-00 00:00:00', '', 'DETECTEUR', 'D7', ''),
(75, 'Terasse : Spot', 'f5ef03d3-75d2-44b3-bda9-93a7ffca4296', '', '0000-00-00 00:00:00', '0', 'LAMPE', 'L15', ''),
(76, 'Terasse : Spot ambiance', 'a68ccb2c-05f7-40eb-9187-99698910f68f', '', '0000-00-00 00:00:00', '0', 'LAMPE', 'L14', ''),
(77, 'Terasse : temperature', 'da7a3b26-bde2-4e7f-8b4e-be8d2c6e053b', '', '0000-00-00 00:00:00', '0', 'TEMPERATURE', '36612', '');

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

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

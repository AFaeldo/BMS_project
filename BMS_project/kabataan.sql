-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Nov 29, 2025 at 09:57 PM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `kabataan`
--

-- --------------------------------------------------------

--
-- Table structure for table `barangay`
--

CREATE TABLE `barangay` (
  `Barangay_ID` int(11) NOT NULL,
  `Barangay_Name` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `barangay`
--

INSERT INTO `barangay` (`Barangay_ID`, `Barangay_Name`) VALUES
(1, 'Balingayan'),
(2, 'Balite'),
(3, 'Baruyan'),
(4, 'Batino'),
(5, 'Bayanan I'),
(6, 'Bayanan II'),
(7, 'Biga'),
(8, 'Bondoc'),
(9, 'Bucayao'),
(10, 'Buhuan'),
(11, 'Bulusan'),
(12, 'Calero'),
(13, 'Camansihan'),
(14, 'Camilmil'),
(15, 'Canubing I'),
(16, 'Canubing II'),
(17, 'Comunal'),
(18, 'Guinobatan'),
(19, 'Gulod'),
(20, 'Gutad'),
(21, 'Ibaba East'),
(22, 'Ibaba West'),
(23, 'Ilaya'),
(24, 'Lalud'),
(25, 'Lazareto'),
(26, 'Libis'),
(27, 'Lumangbayan'),
(28, 'Mahal Na Pangalan'),
(29, 'Maidlang'),
(30, 'Malad'),
(31, 'Malamig'),
(32, 'Managpi'),
(33, 'Masipit'),
(34, 'Nag-Iba I'),
(35, 'Nag-Iba II'),
(36, 'Navotas'),
(37, 'Pachoca'),
(38, 'Palhi'),
(39, 'Panggalaan'),
(40, 'Parang'),
(41, 'Patas'),
(42, 'Personas'),
(43, 'Putingtubig'),
(44, 'San Antonio'),
(45, 'San Raphael'),
(46, 'San Vicente Central'),
(47, 'San Vicente East'),
(48, 'San Vicente North'),
(49, 'San Vicente South'),
(50, 'San Vicente West'),
(51, 'Sapul'),
(52, 'Silonay'),
(53, 'Sta. Cruz'),
(54, 'Sta. Isabel'),
(55, 'Sta. Maria Village'),
(56, 'Sta. Rita'),
(57, 'Sto. Nino'),
(58, 'Suqui'),
(59, 'Tawagan'),
(60, 'Tawiran'),
(61, 'Tibag'),
(62, 'Wawa');

-- --------------------------------------------------------

--
-- Table structure for table `budget`
--

CREATE TABLE `budget` (
  `budget_id` int(11) NOT NULL,
  `Barangay_ID` int(11) NOT NULL,
  `budget` decimal(10,2) NOT NULL,
  `disbursed` decimal(10,2) NOT NULL,
  `balance` decimal(10,2) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `budget`
--

INSERT INTO `budget` (`budget_id`, `Barangay_ID`, `budget`, `disbursed`, `balance`) VALUES
(1, 7, 1050000.00, 0.00, 1050000.00),
(2, 17, 50000000.00, 0.00, 50000000.00),
(3, 16, 1000000.00, 0.00, 1000000.00),
(4, 5, 2000000.00, 0.00, 2000000.00);

-- --------------------------------------------------------

--
-- Table structure for table `file_upload`
--

CREATE TABLE `file_upload` (
  `File_ID` int(11) NOT NULL,
  `File_Name` varchar(255) NOT NULL,
  `File` varchar(255) NOT NULL,
  `Project_ID` int(11) DEFAULT NULL,
  `User_ID` int(11) DEFAULT NULL,
  `Timestamp` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `login`
--

CREATE TABLE `login` (
  `id` int(11) NOT NULL,
  `username` varchar(50) NOT NULL,
  `password` text NOT NULL,
  `Role_ID` int(11) NOT NULL,
  `User_ID` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `login`
--

INSERT INTO `login` (`id`, `username`, `password`, `Role_ID`, `User_ID`) VALUES
(5, 'SK0001', 'AQAAAAIAAYagAAAAEMg1nQ7sImaiG9dG7xjeRhR1W0ci+IiDltOARI2ZQXE4DWDLUvlZtaOns70IHjqYig==', 1, 5),
(6, 'SK0002', 'AQAAAAIAAYagAAAAEBQnVePJDQ0MztACXLlvdGOcfmgR6ssXXJqi4jEk5rO4npn3iWIbe/gVV8rlY1MVSw==', 3, 6),
(7, 'SK0003', 'AQAAAAIAAYagAAAAEN094Oxib6sq8Y6QswEPoVhqFXpUV+XmQKuq9MgLQulXpRzxIeZqWfNwHojvWTnipQ==', 2, 7),
(9, 'SK0004', 'AQAAAAIAAYagAAAAEBD/mpOOVTKybSilgkzc3zqfKV+J7Kk7SqkNAxHcqTfbbChYXJ7xkltEMmk8k8lwog==', 3, 9);

-- --------------------------------------------------------

--
-- Table structure for table `project`
--

CREATE TABLE `project` (
  `Project_ID` int(11) NOT NULL,
  `User_ID` int(11) DEFAULT NULL,
  `Project_Title` varchar(255) NOT NULL,
  `Project_Description` text DEFAULT NULL,
  `Date_Submitted` date DEFAULT NULL,
  `Project_Status` enum('Pending','Approved','Rejected','Completed') DEFAULT 'Pending',
  `Start_Date` date DEFAULT NULL,
  `End_Date` date DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `project_allocation`
--

CREATE TABLE `project_allocation` (
  `Allocation_ID` int(11) NOT NULL,
  `Budget_ID` int(11) NOT NULL,
  `Project_ID` int(11) NOT NULL,
  `Amount_Allocated` decimal(10,2) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `project_log`
--

CREATE TABLE `project_log` (
  `Log_ID` int(11) NOT NULL,
  `Project_ID` int(11) DEFAULT NULL,
  `User_ID` int(11) DEFAULT NULL,
  `Status` enum('Pending','Approved','Rejected','Completed') DEFAULT 'Pending',
  `Changed_On` datetime DEFAULT current_timestamp(),
  `Remarks` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `role`
--

CREATE TABLE `role` (
  `Role_ID` int(11) NOT NULL,
  `Role_Name` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `role`
--

INSERT INTO `role` (`Role_ID`, `Role_Name`) VALUES
(1, 'SuperAdmin'),
(2, 'FederationPresident\n'),
(3, 'BarangaySK');

-- --------------------------------------------------------

--
-- Table structure for table `user`
--

CREATE TABLE `user` (
  `User_ID` int(11) NOT NULL,
  `First_Name` varchar(100) NOT NULL,
  `Last_Name` varchar(100) NOT NULL,
  `Email` varchar(150) DEFAULT NULL,
  `Barangay_ID` int(11) DEFAULT NULL,
  `Role_ID` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `user`
--

INSERT INTO `user` (`User_ID`, `First_Name`, `Last_Name`, `Email`, `Barangay_ID`, `Role_ID`) VALUES
(5, 'Brent Paulos', 'Bolanos', 'ifdhjbjkfhb@gmail.com', 7, 1),
(6, 'Brent', 'Dela Cruz', 'ifdhjbjkfhb@gmail.com', 16, 3),
(7, 'Juan', 'Dela Cruz', 'ifdhjbjkfhb@gmail.com', 19, 2),
(9, 'Juan', 'Dela Cruz', 'ifdhjbjkfhb@gmail.com', 5, 3);

-- --------------------------------------------------------

--
-- Table structure for table `youth_member`
--

CREATE TABLE `youth_member` (
  `Member_ID` int(11) NOT NULL,
  `First_Name` varchar(100) NOT NULL,
  `Last_Name` varchar(100) NOT NULL,
  `Age` int(11) DEFAULT NULL,
  `Gender` enum('Male','Female','Other') DEFAULT NULL,
  `sitio` varchar(255) NOT NULL,
  `Birthday` date NOT NULL,
  `Barangay_ID` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `barangay`
--
ALTER TABLE `barangay`
  ADD PRIMARY KEY (`Barangay_ID`);

--
-- Indexes for table `budget`
--
ALTER TABLE `budget`
  ADD PRIMARY KEY (`budget_id`),
  ADD KEY `fk_budget_barangay` (`Barangay_ID`);

--
-- Indexes for table `file_upload`
--
ALTER TABLE `file_upload`
  ADD PRIMARY KEY (`File_ID`),
  ADD KEY `Project_ID` (`Project_ID`),
  ADD KEY `User_ID` (`User_ID`);

--
-- Indexes for table `login`
--
ALTER TABLE `login`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `username` (`username`),
  ADD KEY `Role` (`Role_ID`),
  ADD KEY `fk_login_user` (`User_ID`);

--
-- Indexes for table `project`
--
ALTER TABLE `project`
  ADD PRIMARY KEY (`Project_ID`),
  ADD KEY `User_ID` (`User_ID`);

--
-- Indexes for table `project_allocation`
--
ALTER TABLE `project_allocation`
  ADD PRIMARY KEY (`Allocation_ID`),
  ADD KEY `fk_alloc_budget` (`Budget_ID`),
  ADD KEY `fk_alloc_project` (`Project_ID`);

--
-- Indexes for table `project_log`
--
ALTER TABLE `project_log`
  ADD PRIMARY KEY (`Log_ID`),
  ADD KEY `Project_ID` (`Project_ID`),
  ADD KEY `User_ID` (`User_ID`);

--
-- Indexes for table `role`
--
ALTER TABLE `role`
  ADD PRIMARY KEY (`Role_ID`);

--
-- Indexes for table `user`
--
ALTER TABLE `user`
  ADD PRIMARY KEY (`User_ID`),
  ADD KEY `Barangay_ID` (`Barangay_ID`),
  ADD KEY `Role_ID` (`Role_ID`);

--
-- Indexes for table `youth_member`
--
ALTER TABLE `youth_member`
  ADD PRIMARY KEY (`Member_ID`),
  ADD KEY `barangay_id` (`Barangay_ID`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `barangay`
--
ALTER TABLE `barangay`
  MODIFY `Barangay_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=63;

--
-- AUTO_INCREMENT for table `budget`
--
ALTER TABLE `budget`
  MODIFY `budget_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `file_upload`
--
ALTER TABLE `file_upload`
  MODIFY `File_ID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `login`
--
ALTER TABLE `login`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT for table `project`
--
ALTER TABLE `project`
  MODIFY `Project_ID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `project_allocation`
--
ALTER TABLE `project_allocation`
  MODIFY `Allocation_ID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `project_log`
--
ALTER TABLE `project_log`
  MODIFY `Log_ID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `role`
--
ALTER TABLE `role`
  MODIFY `Role_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `user`
--
ALTER TABLE `user`
  MODIFY `User_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT for table `youth_member`
--
ALTER TABLE `youth_member`
  MODIFY `Member_ID` int(11) NOT NULL AUTO_INCREMENT;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `budget`
--
ALTER TABLE `budget`
  ADD CONSTRAINT `fk_budget_barangay` FOREIGN KEY (`Barangay_ID`) REFERENCES `barangay` (`Barangay_ID`) ON UPDATE CASCADE;

--
-- Constraints for table `file_upload`
--
ALTER TABLE `file_upload`
  ADD CONSTRAINT `file_upload_ibfk_1` FOREIGN KEY (`Project_ID`) REFERENCES `project` (`Project_ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `file_upload_ibfk_2` FOREIGN KEY (`User_ID`) REFERENCES `user` (`User_ID`) ON DELETE SET NULL ON UPDATE CASCADE;

--
-- Constraints for table `login`
--
ALTER TABLE `login`
  ADD CONSTRAINT `Role` FOREIGN KEY (`Role_ID`) REFERENCES `role` (`Role_ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_login_user` FOREIGN KEY (`User_ID`) REFERENCES `user` (`User_ID`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `project`
--
ALTER TABLE `project`
  ADD CONSTRAINT `project_ibfk_1` FOREIGN KEY (`User_ID`) REFERENCES `user` (`User_ID`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `project_allocation`
--
ALTER TABLE `project_allocation`
  ADD CONSTRAINT `fk_alloc_budget` FOREIGN KEY (`Budget_ID`) REFERENCES `budget` (`budget_id`) ON DELETE CASCADE,
  ADD CONSTRAINT `fk_alloc_project` FOREIGN KEY (`Project_ID`) REFERENCES `project` (`Project_ID`) ON DELETE CASCADE;

--
-- Constraints for table `project_log`
--
ALTER TABLE `project_log`
  ADD CONSTRAINT `project_log_ibfk_1` FOREIGN KEY (`Project_ID`) REFERENCES `project` (`Project_ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `project_log_ibfk_2` FOREIGN KEY (`User_ID`) REFERENCES `user` (`User_ID`) ON DELETE SET NULL ON UPDATE CASCADE;

--
-- Constraints for table `user`
--
ALTER TABLE `user`
  ADD CONSTRAINT `user_ibfk_1` FOREIGN KEY (`Barangay_ID`) REFERENCES `barangay` (`Barangay_ID`) ON DELETE SET NULL ON UPDATE CASCADE,
  ADD CONSTRAINT `user_ibfk_2` FOREIGN KEY (`Role_ID`) REFERENCES `role` (`Role_ID`) ON DELETE SET NULL ON UPDATE CASCADE;

--
-- Constraints for table `youth_member`
--
ALTER TABLE `youth_member`
  ADD CONSTRAINT `barangay_id` FOREIGN KEY (`Barangay_ID`) REFERENCES `barangay` (`Barangay_ID`) ON DELETE SET NULL,
  ADD CONSTRAINT `youth_member_ibfk_1` FOREIGN KEY (`Barangay_ID`) REFERENCES `barangay` (`Barangay_ID`) ON DELETE SET NULL ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

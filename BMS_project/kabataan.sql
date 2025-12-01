-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Dec 01, 2025 at 05:18 PM
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
(3, 16, 1000000.00, 50000.00, 950000.00),
(4, 5, 2000000.00, 1350050.00, 649950.00),
(5, 2, 5000000.00, 0.00, 5000000.00);

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

--
-- Dumping data for table `file_upload`
--

INSERT INTO `file_upload` (`File_ID`, `File_Name`, `File`, `Project_ID`, `User_ID`, `Timestamp`) VALUES
(1, 'Proposal', '/UploadedFiles/Proposal_20251130053905.pdf', 1, 9, '2025-11-30 05:39:05'),
(2, 'adssdadsasad', '/UploadedFiles/adssdadsasad_20251130055843.pdf', 2, 9, '2025-11-30 05:58:43'),
(3, 'Proposal', '/UploadedFiles/Proposal_20251130061050.pdf', 3, 6, '2025-11-30 06:10:50'),
(4, 'PrefinalExam', '/UploadedFiles/PrefinalExam_20251130110427.pdf', 4, 9, '2025-11-30 11:04:27'),
(5, 'Sample', '/UploadedFiles/Sample_20251130162944_669bed.pdf', 10, 9, '2025-11-30 16:29:44'),
(6, 'Sampledasf ', '/UploadedFiles/Sampledasf _20251130212219_802b38.pdf', 11, 9, '2025-11-30 21:22:19'),
(7, 'Sampledasf ', '/UploadedFiles/Sampledasf _20251130234116_88a554.pdf', 12, 9, '2025-11-30 23:41:16');

-- --------------------------------------------------------

--
-- Table structure for table `kabataan_service_record`
--

CREATE TABLE `kabataan_service_record` (
  `Record_ID` int(11) NOT NULL,
  `User_ID` int(11) NOT NULL,
  `Term_ID` int(11) DEFAULT NULL,
  `Role_ID` int(11) NOT NULL,
  `Status` enum('Active','Resigned','Term Ended') DEFAULT 'Active',
  `Actual_End_Date` date DEFAULT NULL COMMENT 'Only set if user resigns before term ends'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `kabataan_service_record`
--

INSERT INTO `kabataan_service_record` (`Record_ID`, `User_ID`, `Term_ID`, `Role_ID`, `Status`, `Actual_End_Date`) VALUES
(1, 5, NULL, 1, 'Active', NULL),
(2, 7, 2, 2, 'Resigned', '2025-12-01'),
(3, 6, 2, 3, 'Resigned', '2025-12-01'),
(4, 9, 2, 3, 'Resigned', '2025-12-01'),
(5, 10, 2, 3, 'Resigned', '2025-12-01'),
(6, 6, 3, 3, 'Resigned', '2025-12-01'),
(7, 7, 3, 2, 'Resigned', '2025-12-01'),
(8, 9, 3, 3, 'Resigned', '2025-12-01'),
(9, 10, 3, 3, 'Resigned', '2025-12-01'),
(10, 11, 3, 3, 'Resigned', '2025-12-01'),
(11, 10, 3, 3, 'Resigned', '2025-12-01'),
(12, 6, 4, 3, 'Term Ended', NULL),
(13, 7, 4, 2, 'Term Ended', NULL),
(14, 9, 4, 3, 'Term Ended', NULL),
(15, 10, 4, 3, 'Active', NULL),
(16, 11, 4, 3, 'Active', NULL),
(17, 6, 5, 3, 'Active', NULL),
(18, 7, 5, 2, 'Active', NULL),
(19, 9, 5, 3, 'Resigned', '2025-12-01'),
(20, 9, 5, 3, 'Active', NULL);

-- --------------------------------------------------------

--
-- Table structure for table `kabataan_term_period`
--

CREATE TABLE `kabataan_term_period` (
  `Term_ID` int(11) NOT NULL,
  `Term_Name` varchar(100) NOT NULL,
  `Start_Date` date NOT NULL,
  `Official_End_Date` date NOT NULL,
  `IsActive` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `kabataan_term_period`
--

INSERT INTO `kabataan_term_period` (`Term_ID`, `Term_Name`, `Start_Date`, `Official_End_Date`, `IsActive`) VALUES
(1, 'Permanent Appointment', '2000-01-01', '2099-12-31', 0),
(2, '2023-2026 SK Term', '2023-11-01', '2025-11-30', 0),
(3, '2025-2028 Term', '2025-12-01', '2028-01-01', 0),
(4, '2021-2024 Term', '2021-01-09', '2024-02-07', 0),
(5, '2025-2028 Term', '2025-12-02', '2028-06-08', 1);

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
(9, 'SK0004', 'AQAAAAIAAYagAAAAEBD/mpOOVTKybSilgkzc3zqfKV+J7Kk7SqkNAxHcqTfbbChYXJ7xkltEMmk8k8lwog==', 3, 9),
(10, 'SK0005', 'AQAAAAIAAYagAAAAEEaXxHIYiIjCebzsz6Y1du+69JkEECn5kxugS8uF3lkdc6UKwQ+3frg+0uhqwT4BxA==', 3, 10),
(11, 'SK0006', 'AQAAAAIAAYagAAAAELRiCqGnXF29qpS1qBMcAzJDh/nkgg06TdQcYJtAndvRuB4/znVJe4kenwcORtJnpw==', 3, 11);

-- --------------------------------------------------------

--
-- Table structure for table `project`
--

CREATE TABLE `project` (
  `Project_ID` int(11) NOT NULL,
  `User_ID` int(11) NOT NULL,
  `Project_Title` varchar(255) NOT NULL,
  `Project_Description` text DEFAULT NULL,
  `Date_Submitted` date NOT NULL,
  `Project_Status` enum('Pending','Approved','Rejected','Completed') NOT NULL DEFAULT 'Pending',
  `Start_Date` date NOT NULL,
  `End_Date` date NOT NULL,
  `IsArchived` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `project`
--

INSERT INTO `project` (`Project_ID`, `User_ID`, `Project_Title`, `Project_Description`, `Date_Submitted`, `Project_Status`, `Start_Date`, `End_Date`, `IsArchived`) VALUES
(1, 9, 'Prefinal', 'sadsdasdas', '2025-11-30', 'Approved', '2025-11-30', '2025-12-07', 0),
(2, 9, 'asdsadsa', 'sdasdasdsad', '2025-11-30', 'Approved', '2025-12-07', '2025-12-14', 0),
(3, 6, 'dassadsad', 'asdasdasdsad', '2025-11-30', 'Approved', '2025-12-01', '2025-12-07', 0),
(4, 9, 'Prefinal', 'Pre Finals', '2025-11-30', 'Approved', '2025-12-01', '2025-12-06', 0),
(10, 9, 'Sample', 'Sample', '2025-11-30', 'Approved', '2025-12-01', '2025-12-07', 0),
(11, 9, 'Sample', 'asdsadsadsad', '2025-11-30', 'Approved', '2025-12-01', '2025-12-07', 0),
(12, 9, 'Sampleasdsda', 'asdsdasadsadsda', '2025-11-30', 'Approved', '2025-12-01', '2025-12-07', 0);

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

--
-- Dumping data for table `project_allocation`
--

INSERT INTO `project_allocation` (`Allocation_ID`, `Budget_ID`, `Project_ID`, `Amount_Allocated`) VALUES
(1, 4, 1, 100000.00),
(2, 4, 2, 50000.00),
(3, 3, 3, 50000.00),
(4, 4, 4, 100000.00),
(5, 4, 10, 1000000.00),
(6, 4, 11, 50.00),
(7, 4, 12, 100000.00);

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

--
-- Dumping data for table `project_log`
--

INSERT INTO `project_log` (`Log_ID`, `Project_ID`, `User_ID`, `Status`, `Changed_On`, `Remarks`) VALUES
(1, 1, 9, 'Pending', '2025-11-30 05:39:05', 'Project created and submitted for approval.'),
(2, 1, 9, 'Approved', '2025-11-30 05:56:31', 'Project status updated to Approved by Federation President'),
(3, 2, 9, 'Pending', '2025-11-30 05:58:43', 'Project created and submitted for approval.'),
(4, 3, 6, 'Pending', '2025-11-30 06:10:50', 'Project created and submitted for approval.'),
(5, 2, 9, 'Pending', '2025-11-30 06:11:17', 'Project status updated to Pending by Federation President'),
(6, 2, 9, 'Approved', '2025-11-30 06:11:24', 'Project status updated to Approved by Federation President'),
(7, 3, 6, 'Approved', '2025-11-30 06:11:35', 'Project status updated to Approved by Federation President'),
(8, 4, 9, 'Pending', '2025-11-30 11:04:27', 'Project created and submitted for approval.'),
(9, 4, 9, 'Pending', '2025-11-30 12:42:03', 'Project status updated to Pending by Federation President'),
(10, 4, 9, 'Pending', '2025-11-30 12:42:09', 'Project status updated to Pending by Federation President'),
(11, 4, 9, 'Pending', '2025-11-30 12:42:12', 'Project status updated to Pending by Federation President'),
(12, 10, 9, 'Pending', '2025-11-30 16:29:44', 'Project created and submitted for approval.'),
(13, 11, 9, 'Pending', '2025-11-30 21:22:19', 'Project created and submitted for approval.'),
(14, 12, 9, 'Pending', '2025-11-30 23:41:16', 'Project created and submitted for approval.'),
(15, 4, 9, 'Pending', '2025-12-01 12:44:55', 'Project status updated to Pending by Federation President'),
(16, 10, 9, 'Approved', '2025-12-01 12:52:54', 'Project status updated to Approved by Federation President'),
(17, 11, 9, 'Pending', '2025-12-01 12:53:14', 'Project status updated to Pending by Federation President'),
(18, 11, 9, 'Pending', '2025-12-01 12:53:18', 'Project status updated to Pending by Federation President'),
(19, 11, 9, 'Pending', '2025-12-01 12:53:37', 'Project status updated to Pending by Federation President'),
(20, 4, 9, 'Approved', '2025-12-01 12:54:07', 'Project status updated to Approved by Federation President'),
(21, 11, 9, 'Pending', '2025-12-01 12:54:43', 'Project status updated to Pending by Federation President'),
(22, 11, 9, 'Approved', '2025-12-01 12:54:57', 'Project status updated to Approved by Federation President'),
(23, 12, 9, 'Pending', '2025-12-01 23:58:44', 'Project status updated to Pending by Federation President'),
(24, 12, 9, 'Approved', '2025-12-01 23:58:51', 'Project status updated to Approved by Federation President');

-- --------------------------------------------------------

--
-- Table structure for table `role`
--

CREATE TABLE `role` (
  `Role_ID` int(11) NOT NULL,
  `Role_Name` varchar(100) NOT NULL,
  `Has_Term_Limit` tinyint(1) DEFAULT 1 COMMENT '1=Elected/Expires, 0=Permanent/Admin'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `role`
--

INSERT INTO `role` (`Role_ID`, `Role_Name`, `Has_Term_Limit`) VALUES
(1, 'SuperAdmin', 0),
(2, 'FederationPresident\n', 1),
(3, 'BarangaySK', 1);

-- --------------------------------------------------------

--
-- Table structure for table `system_log`
--

CREATE TABLE `system_log` (
  `SysLog_id` int(11) NOT NULL,
  `User_ID` int(11) NOT NULL,
  `Action` varchar(50) NOT NULL,
  `Table_Name` varchar(50) DEFAULT NULL,
  `Record_ID` int(11) DEFAULT NULL,
  `Remark` text DEFAULT NULL,
  `DateTime` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `system_log`
--

INSERT INTO `system_log` (`SysLog_id`, `User_ID`, `Action`, `Table_Name`, `Record_ID`, `Remark`, `DateTime`) VALUES
(1, 5, '', NULL, NULL, 'User Logged In', '2025-12-01 23:52:51'),
(2, 5, '', NULL, NULL, 'User Logged In', '2025-12-01 23:56:38'),
(3, 5, '', NULL, NULL, 'Resigned/Archived User: SK0004', '2025-12-01 23:57:06'),
(4, 5, '', NULL, NULL, 'Re-elected User: SK0004', '2025-12-01 23:57:21'),
(5, 6, '', NULL, NULL, 'User Logged In', '2025-12-01 23:57:33'),
(6, 7, '', NULL, NULL, 'User Logged In', '2025-12-01 23:58:18'),
(7, 5, '', NULL, NULL, 'User Logged In', '2025-12-01 23:59:43'),
(8, 5, '', NULL, NULL, 'User Logged In', '2025-12-02 00:05:38'),
(9, 9, '', NULL, NULL, 'User Logged In', '2025-12-02 00:05:55'),
(10, 5, '', NULL, NULL, 'User Logged In', '2025-12-02 00:06:14'),
(11, 9, '', NULL, NULL, 'User Logged In', '2025-12-02 00:09:19'),
(12, 5, '', NULL, NULL, 'User Logged In', '2025-12-02 00:10:39');

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
  `Role_ID` int(11) DEFAULT NULL,
  `IsArchived` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `user`
--

INSERT INTO `user` (`User_ID`, `First_Name`, `Last_Name`, `Email`, `Barangay_ID`, `Role_ID`, `IsArchived`) VALUES
(5, 'Brent Paulos', 'Bolanos', 'ifdhjbjkfhb@gmail.com', 7, 1, 0),
(6, 'Brent', 'Dela Cruz', 'ifdhjbjkfhb@gmail.com', 16, 3, 0),
(7, 'Juan', 'Dela Cruz', 'ifdhjbjkfhb@gmail.com', 19, 2, 0),
(9, 'Juan', 'Dela Cruz', 'ifdhjbjkfhb@gmail.com', 5, 3, 0),
(10, 'Jackolin', 'Jack', 'ralphjohnsales123@gmail.com', 13, 3, 0),
(11, 'Zyris', 'Ortaleza', 'sklnjdb@gmail.com', 58, 3, 0);

-- --------------------------------------------------------

--
-- Table structure for table `youth_member`
--

CREATE TABLE `youth_member` (
  `Member_ID` int(11) NOT NULL,
  `Barangay_ID` int(11) NOT NULL,
  `First_Name` varchar(100) NOT NULL,
  `Last_Name` varchar(100) NOT NULL,
  `Age` int(11) NOT NULL,
  `Gender` enum('Male','Female','Other') NOT NULL,
  `sitio` varchar(255) NOT NULL,
  `Birthday` date NOT NULL,
  `IsArchived` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `youth_member`
--

INSERT INTO `youth_member` (`Member_ID`, `Barangay_ID`, `First_Name`, `Last_Name`, `Age`, `Gender`, `sitio`, `Birthday`, `IsArchived`) VALUES
(4, 5, 'kurt', 'asd', 14, 'Male', 'Kanluran', '2011-06-09', 0),
(5, 16, 'Brent Paulo', 'Bolanos', 21, 'Male', 'Kanluran', '2004-06-07', 0),
(6, 5, 'Zyris', 'Ortaleza', 20, 'Female', 'adsadsadsa', '2005-05-02', 0);

-- --------------------------------------------------------

--
-- Table structure for table `__efmigrationshistory`
--

CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL
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
-- Indexes for table `kabataan_service_record`
--
ALTER TABLE `kabataan_service_record`
  ADD PRIMARY KEY (`Record_ID`),
  ADD KEY `User_ID` (`User_ID`),
  ADD KEY `Term_ID` (`Term_ID`),
  ADD KEY `Role_ID` (`Role_ID`);

--
-- Indexes for table `kabataan_term_period`
--
ALTER TABLE `kabataan_term_period`
  ADD PRIMARY KEY (`Term_ID`);

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
-- Indexes for table `system_log`
--
ALTER TABLE `system_log`
  ADD PRIMARY KEY (`SysLog_id`),
  ADD KEY `fk_User_Id` (`User_ID`);

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
  ADD KEY `youth_member_ibfk_1` (`Barangay_ID`);

--
-- Indexes for table `__efmigrationshistory`
--
ALTER TABLE `__efmigrationshistory`
  ADD PRIMARY KEY (`MigrationId`);

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
  MODIFY `budget_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `file_upload`
--
ALTER TABLE `file_upload`
  MODIFY `File_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT for table `kabataan_service_record`
--
ALTER TABLE `kabataan_service_record`
  MODIFY `Record_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=21;

--
-- AUTO_INCREMENT for table `kabataan_term_period`
--
ALTER TABLE `kabataan_term_period`
  MODIFY `Term_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `login`
--
ALTER TABLE `login`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- AUTO_INCREMENT for table `project`
--
ALTER TABLE `project`
  MODIFY `Project_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT for table `project_allocation`
--
ALTER TABLE `project_allocation`
  MODIFY `Allocation_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT for table `project_log`
--
ALTER TABLE `project_log`
  MODIFY `Log_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=25;

--
-- AUTO_INCREMENT for table `role`
--
ALTER TABLE `role`
  MODIFY `Role_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `system_log`
--
ALTER TABLE `system_log`
  MODIFY `SysLog_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT for table `user`
--
ALTER TABLE `user`
  MODIFY `User_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- AUTO_INCREMENT for table `youth_member`
--
ALTER TABLE `youth_member`
  MODIFY `Member_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

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
-- Constraints for table `kabataan_service_record`
--
ALTER TABLE `kabataan_service_record`
  ADD CONSTRAINT `fk_service_role` FOREIGN KEY (`Role_ID`) REFERENCES `role` (`Role_ID`),
  ADD CONSTRAINT `fk_service_term` FOREIGN KEY (`Term_ID`) REFERENCES `kabataan_term_period` (`Term_ID`),
  ADD CONSTRAINT `fk_service_user` FOREIGN KEY (`User_ID`) REFERENCES `user` (`User_ID`) ON DELETE CASCADE;

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
-- Constraints for table `system_log`
--
ALTER TABLE `system_log`
  ADD CONSTRAINT `fk_User_Id` FOREIGN KEY (`User_ID`) REFERENCES `user` (`User_ID`);

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
  ADD CONSTRAINT `barangay_id` FOREIGN KEY (`Barangay_ID`) REFERENCES `barangay` (`Barangay_ID`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

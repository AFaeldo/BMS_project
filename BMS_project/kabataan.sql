-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Dec 04, 2025 at 12:15 AM
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
-- Table structure for table `announcement`
--

CREATE TABLE `announcement` (
  `Announcement_ID` int(11) NOT NULL,
  `User_ID` int(11) NOT NULL,
  `Title` varchar(255) NOT NULL,
  `Message` text NOT NULL,
  `IsActive` tinyint(1) DEFAULT 1 COMMENT '1=Show on Login, 0=Hidden',
  `Date_Created` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

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
  `balance` decimal(10,2) NOT NULL,
  `Term_ID` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `budget`
--

INSERT INTO `budget` (`budget_id`, `Barangay_ID`, `budget`, `disbursed`, `balance`, `Term_ID`) VALUES
(7, 5, 500000.00, 250.00, 499750.00, 5),
(8, 5, 25000.00, 250.00, 24750.00, 6),
(9, 5, 100000.00, 0.00, 100000.00, 8);

-- --------------------------------------------------------

--
-- Table structure for table `compliance`
--

CREATE TABLE `compliance` (
  `com_id` int(11) NOT NULL,
  `Barangay_id` int(11) NOT NULL,
  `File_ID` int(11) DEFAULT NULL,
  `Title` varchar(255) NOT NULL,
  `Type` varchar(255) NOT NULL,
  `Status` varchar(50) NOT NULL,
  `due_date` date NOT NULL,
  `Date_Submitted` datetime DEFAULT NULL,
  `Term_ID` int(11) NOT NULL,
  `IsArchived` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `compliance`
--

INSERT INTO `compliance` (`com_id`, `Barangay_id`, `File_ID`, `Title`, `Type`, `Status`, `due_date`, `Date_Submitted`, `Term_ID`, `IsArchived`) VALUES
(9, 1, NULL, 'Secret', 'Report', 'Not Submitted', '2025-12-27', NULL, 6, 0),
(10, 3, NULL, 'Secretasdsadsadsa', 'Minutes', 'Not Submitted', '2025-12-27', NULL, 6, 0),
(11, 3, NULL, 'Secretasdsadsadsa', 'Minutes', 'Not Submitted', '2025-12-27', NULL, 8, 0),
(12, 1, NULL, 'Secretasdsadsadsa', 'Minutes of Meeting', 'Not Submitted', '2025-12-27', NULL, 8, 0);

-- --------------------------------------------------------

--
-- Table structure for table `federation_fund`
--

CREATE TABLE `federation_fund` (
  `Fund_ID` int(11) NOT NULL,
  `Term_ID` int(11) NOT NULL,
  `Total_Amount` decimal(20,2) NOT NULL DEFAULT 0.00,
  `Allocated_To_Barangays` decimal(20,2) NOT NULL DEFAULT 0.00,
  `Created_At` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `federation_fund`
--

INSERT INTO `federation_fund` (`Fund_ID`, `Term_ID`, `Total_Amount`, `Allocated_To_Barangays`, `Created_At`) VALUES
(1, 5, 1000000.00, 500000.00, '2025-12-03 06:43:29'),
(2, 3, 500000.00, 0.00, '2025-12-03 11:20:17'),
(3, 6, 1000000.00, 25000.00, '2025-12-04 04:18:38'),
(4, 8, 100000.00, 100000.00, '2025-12-04 06:28:54');

-- --------------------------------------------------------

--
-- Table structure for table `file_upload`
--

CREATE TABLE `file_upload` (
  `File_ID` int(11) NOT NULL,
  `File_Name` varchar(255) NOT NULL,
  `File` varchar(255) NOT NULL,
  `User_ID` int(11) DEFAULT NULL,
  `Timestamp` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `file_upload`
--

INSERT INTO `file_upload` (`File_ID`, `File_Name`, `File`, `User_ID`, `Timestamp`) VALUES
(9, 'LabExercise5.pdf', '/UploadedFiles/Instructions/LabExercise5_20251203224815.pdf', 7, '2025-12-03 22:48:15'),
(10, 'LabExercise5.pdf', '/UploadedFiles/Instructions/LabExercise5_20251203225605.pdf', 7, '2025-12-03 22:56:05'),
(11, 'Exercise 8.pdf', '/UploadedFiles/Instructions/Exercise 8_20251203225721.pdf', 7, '2025-12-03 22:57:21'),
(12, 'Exercise 8.pdf', '/UploadedFiles/Submissions/4_20251204012105_209e6c.pdf', 9, '2025-12-04 01:21:05'),
(13, 'Sampledasf ', '/UploadedFiles/Sampledasf _20251204043256_739dbd.pdf', 9, '2025-12-04 04:32:56');

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
(15, 10, 4, 3, 'Term Ended', '2025-12-03'),
(16, 11, 4, 3, 'Resigned', '2025-12-02'),
(17, 6, 5, 3, 'Term Ended', '2025-12-04'),
(18, 7, 5, 2, 'Resigned', '2025-12-02'),
(19, 9, 5, 3, 'Resigned', '2025-12-01'),
(20, 9, 5, 3, 'Term Ended', '2025-12-03'),
(21, 7, 5, 2, 'Term Ended', '2025-12-03'),
(22, 11, 5, 3, 'Active', NULL),
(23, 9, 6, 3, 'Term Ended', '2025-12-04'),
(24, 7, 6, 2, 'Resigned', '2025-12-04'),
(25, 6, 6, 3, 'Resigned', '2025-12-04'),
(26, 10, 6, 3, 'Active', NULL),
(27, 9, 8, 3, 'Active', NULL),
(28, 6, 8, 3, 'Active', NULL),
(29, 7, 8, 2, 'Active', NULL);

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
(1, 'Permanent Appointment', '0000-00-00', '0000-00-00', 0),
(2, '2023-2026 SK Term', '2023-11-01', '2025-11-30', 0),
(3, '2025-2028 Term', '2025-12-01', '2028-01-01', 0),
(4, '2021-2024 Term', '2021-01-09', '2024-02-07', 0),
(5, '2025-2028 Term', '2025-12-02', '2028-06-09', 0),
(6, '2028-2031 Term', '2028-06-23', '2031-07-18', 0),
(7, '2021-2024 Term', '2021-02-01', '2024-02-03', 0),
(8, '2031-2034 term', '2031-07-11', '2034-08-12', 1);

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
(5, 'SK0001', 'AQAAAAIAAYagAAAAEAMz2aCc7Rkb6L6PQmDueww7yjqjItSaxOmeOHKUiJB+Njb554gvn1Coa6sXOihatg==', 1, 5),
(6, 'SK0002', 'AQAAAAIAAYagAAAAELl1++Eer7Tl0byioPYOLacTOe0mzK5eO0ldpEq02Yzw3xO6urWfALR54+c/RZnfrQ==', 3, 6),
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
  `Term_ID` int(11) DEFAULT NULL,
  `File_ID` int(11) NOT NULL,
  `Project_Title` varchar(255) NOT NULL,
  `Project_Description` text DEFAULT NULL,
  `Estimated_Cost` decimal(18,2) NOT NULL DEFAULT 0.00,
  `Date_Submitted` date NOT NULL,
  `Project_Status` enum('Pending','Approved','Rejected','Completed') NOT NULL DEFAULT 'Pending',
  `Start_Date` date NOT NULL,
  `End_Date` date NOT NULL,
  `IsArchived` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `project`
--

INSERT INTO `project` (`Project_ID`, `User_ID`, `Term_ID`, `File_ID`, `Project_Title`, `Project_Description`, `Estimated_Cost`, `Date_Submitted`, `Project_Status`, `Start_Date`, `End_Date`, `IsArchived`) VALUES
(16, 9, 7, 13, 'Sampleasdsda', 'asdsadsadsa', 0.00, '2025-12-04', 'Rejected', '2025-12-01', '2025-12-07', 1);

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
(2, 8, 16, 250.00),
(3, 9, 16, 0.00),
(4, 9, 16, 0.00);

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
(1, 16, 9, 'Pending', '2025-12-04 04:32:56', 'Project created and submitted for approval.'),
(2, 16, 9, 'Approved', '2025-12-04 05:53:08', 'Project status updated to Approved by Federation President');

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
-- Table structure for table `sitio`
--

CREATE TABLE `sitio` (
  `sitio_id` int(11) NOT NULL,
  `Barangay_id` int(11) DEFAULT NULL,
  `Name` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `sitio`
--

INSERT INTO `sitio` (`sitio_id`, `Barangay_id`, `Name`) VALUES
(1, 5, 'Kanluran'),
(2, 5, 'Hilaga'),
(3, 5, 'weh');

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
(12, 5, '', NULL, NULL, 'User Logged In', '2025-12-02 00:10:39'),
(13, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-02 00:31:37'),
(14, 9, 'Archive Youth', 'YouthMember', 4, 'Archived Youth: kurt asd', '2025-12-02 00:31:44'),
(15, 9, 'Restore Youth', 'YouthMember', NULL, 'Restored 1 Youth Members', '2025-12-02 00:31:47'),
(16, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-02 00:32:08'),
(17, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-02 00:33:56'),
(18, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-02 21:21:02'),
(19, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-02 22:16:05'),
(20, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-02 22:34:45'),
(21, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-02 22:41:23'),
(22, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-02 22:44:52'),
(23, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-02 23:09:33'),
(24, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-02 23:14:24'),
(25, 5, 'Archive User', 'User', 7, 'Resigned/Archived User: SK0003', '2025-12-02 23:14:29'),
(26, 5, 'Restore User', 'User', 7, 'Re-elected User: SK0003', '2025-12-02 23:14:35'),
(27, 6, 'Login', NULL, NULL, 'User Logged In', '2025-12-02 23:29:08'),
(28, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-02 23:29:28'),
(29, 5, 'Archive User', 'User', 11, 'Resigned/Archived User: SK0006', '2025-12-02 23:31:43'),
(30, 5, 'Restore User', 'User', 11, 'Re-elected User: SK0006', '2025-12-02 23:31:52'),
(31, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 00:04:46'),
(32, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 00:05:17'),
(33, 6, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 00:06:37'),
(34, 6, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 00:25:15'),
(35, 6, 'Terminate Project', 'Project', 3, 'Terminated Project: dassadsad', '2025-12-03 00:25:22'),
(36, 6, 'Carry Over Project', 'Project', 3, 'Carried Over Project: dassadsad to Term 2025-2028 Term', '2025-12-03 00:42:37'),
(37, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 06:38:27'),
(38, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 06:42:33'),
(39, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 06:54:47'),
(40, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 07:06:56'),
(41, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 07:13:11'),
(42, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 07:22:21'),
(43, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 07:29:17'),
(44, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 07:32:16'),
(45, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 08:19:49'),
(46, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 08:43:40'),
(47, 7, 'Add Budget', 'Budget', 0, 'Added XDR1,500,000.00 Budget to Bayanan II', '2025-12-03 09:03:02'),
(48, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 09:09:07'),
(49, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 09:57:56'),
(50, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 09:58:18'),
(51, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 09:59:25'),
(52, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 09:59:45'),
(53, 7, 'Add Budget', 'Budget', 0, 'Allocated XDR500,000.00 to Bayanan I', '2025-12-03 09:59:56'),
(54, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 10:01:28'),
(55, 9, 'Create Project', 'Project', 13, 'Created Project: Sampleasdsda', '2025-12-03 10:03:51'),
(56, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 10:05:08'),
(57, 7, 'Approve/Reject Project', 'Project', 13, 'Project Sampleasdsda Status Updated to Approved', '2025-12-03 10:05:20'),
(58, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 10:06:54'),
(59, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 10:16:23'),
(60, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 10:26:22'),
(61, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 10:26:22'),
(62, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 10:39:51'),
(63, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 10:42:05'),
(65, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 11:06:36'),
(66, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 11:15:20'),
(67, 5, 'Edit Term', 'Term', 5, 'Updated Term: 2025-2028 Term', '2025-12-03 11:15:50'),
(68, 5, 'Edit Term', 'Term', 5, 'Updated Term: 2025-2028 Term', '2025-12-03 11:18:01'),
(69, 5, 'Set Active Term', 'Term', 3, 'Set Active Term to: 2025-2028 Term', '2025-12-03 11:18:24'),
(70, 5, 'Set Active Term', 'Term', 5, 'Set Active Term to: 2025-2028 Term', '2025-12-03 11:18:36'),
(71, 5, 'Set Active Term', 'Term', 3, 'Set Active Term to: 2025-2028 Term', '2025-12-03 11:20:10'),
(72, 5, 'Set Active Term', 'Term', 5, 'Set Active Term to: 2025-2028 Term', '2025-12-03 11:20:19'),
(73, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 11:30:21'),
(74, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 11:32:25'),
(75, 5, 'Add Term', 'KabataanTermPeriod', 6, 'Added New Term: 2028-2031 Term', '2025-12-03 11:32:32'),
(76, 5, 'Set Active Term', 'Term', 6, 'Set Active Term to: 2028-2031 Term', '2025-12-03 11:32:40'),
(77, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 11:32:54'),
(78, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 11:46:11'),
(79, 5, 'Set Active Term', 'Term', 5, 'Set Active Term to: 2025-2028 Term', '2025-12-03 11:46:26'),
(80, 5, 'Set Active Term', 'Term', 6, 'Set Active Term to: 2028-2031 Term', '2025-12-03 11:46:29'),
(81, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 11:46:49'),
(82, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 11:49:07'),
(83, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 11:49:48'),
(84, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 11:53:50'),
(85, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 11:54:03'),
(86, 6, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 11:54:27'),
(87, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 11:54:42'),
(88, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 12:03:54'),
(89, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 12:04:16'),
(90, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 12:16:58'),
(91, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 12:21:46'),
(92, 5, 'Restore User', 'User', 9, 'Re-elected User: SK0004', '2025-12-03 12:21:51'),
(93, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 12:22:16'),
(94, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 18:52:40'),
(95, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 19:06:48'),
(96, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 19:17:18'),
(97, 5, 'Restore User', 'User', 7, 'Re-elected User: SK0003', '2025-12-03 19:17:55'),
(98, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 19:18:21'),
(99, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 20:11:59'),
(100, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 20:12:24'),
(101, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 20:18:18'),
(102, 5, 'Set Active Term', 'Term', 5, 'Set Active Term to: 2025-2028 Term', '2025-12-03 20:20:30'),
(103, 5, 'Set Active Term', 'Term', 3, 'Set Active Term to: 2025-2028 Term', '2025-12-03 20:20:34'),
(104, 5, 'Set Active Term', 'Term', 6, 'Set Active Term to: 2028-2031 Term', '2025-12-03 20:20:37'),
(105, 5, 'Set Active Term', 'Term', 2, 'Set Active Term to: 2023-2026 SK Term', '2025-12-03 20:25:48'),
(106, 5, 'Set Active Term', 'Term', 6, 'Set Active Term to: 2028-2031 Term', '2025-12-03 20:25:52'),
(107, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 20:27:03'),
(108, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 20:29:53'),
(109, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 20:31:52'),
(110, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 20:35:43'),
(111, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 20:38:32'),
(112, 5, 'Set Active Term', 'Term', 5, 'Set Active Term to: 2025-2028 Term', '2025-12-03 20:38:43'),
(113, 5, 'Set Active Term', 'Term', 6, 'Set Active Term to: 2028-2031 Term', '2025-12-03 20:38:57'),
(114, 5, 'Edit Term', 'Term', 6, 'Updated Term: 2028-2031 Term', '2025-12-03 20:39:20'),
(115, 5, 'Add Term', 'KabataanTermPeriod', 7, 'Added New Term: 2021-2024 Term', '2025-12-03 20:39:56'),
(116, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 20:40:43'),
(117, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 20:41:10'),
(118, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 20:42:24'),
(119, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 20:45:13'),
(120, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 20:45:45'),
(121, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 20:52:08'),
(122, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 20:58:28'),
(123, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 21:27:36'),
(124, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 21:34:03'),
(125, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 21:40:21'),
(126, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 21:40:37'),
(127, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 21:52:15'),
(128, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 21:52:32'),
(129, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 21:52:42'),
(130, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 22:00:56'),
(131, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 22:06:08'),
(132, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 22:10:36'),
(133, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 22:17:42'),
(134, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 22:26:07'),
(135, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 22:47:27'),
(136, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 22:47:39'),
(137, 9, 'Archive Youth', 'YouthMember', 4, 'Archived Youth: kurt asd', '2025-12-03 22:47:44'),
(138, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 22:47:58'),
(139, 7, 'Create Compliance', NULL, NULL, 'Created new requirement: Liga Report', '2025-12-03 22:48:15'),
(140, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 22:55:40'),
(141, 7, 'Create Compliance', NULL, NULL, 'Created new requirement: Liga Report', '2025-12-03 22:56:05'),
(142, 7, 'Create Compliance', NULL, NULL, 'Created new requirement: Liga Reportasdasdasdsad', '2025-12-03 22:57:21'),
(143, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 23:18:00'),
(144, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 23:21:31'),
(145, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 23:28:11'),
(146, 7, 'Create Compliance', NULL, NULL, 'Created new requirement: Liga Reportasdasdasdsad', '2025-12-03 23:28:26'),
(147, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 23:37:28'),
(148, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 23:55:02'),
(149, 7, 'Create Compliance', NULL, NULL, 'Created new requirement: ayaw ko na', '2025-12-03 23:55:21'),
(150, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-03 23:58:35'),
(151, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 00:04:57'),
(152, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 00:07:11'),
(153, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 00:13:37'),
(154, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 00:16:32'),
(155, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 00:25:17'),
(156, 7, 'Create Compliance', 'Compliance', 6, 'Created requirement: asdasdasdasd', '2025-12-04 00:25:31'),
(157, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 00:27:14'),
(158, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 00:34:52'),
(159, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 00:38:54'),
(160, 7, 'Archive Compliance', 'Compliance', 3, 'Archived requirement: Liga Reportasdasdasdsad', '2025-12-04 00:48:00'),
(161, 7, 'Archive Compliance', 'Compliance', 6, 'Archived requirement: asdasdasdasd', '2025-12-04 00:49:07'),
(162, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 00:53:03'),
(163, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 00:56:07'),
(164, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 00:57:41'),
(165, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 00:58:20'),
(166, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 01:00:41'),
(167, 7, 'Create Compliance', 'Compliance', 7, 'Created requirement: Secret', '2025-12-04 01:01:05'),
(168, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 01:03:38'),
(169, 7, 'Create Compliance', 'Compliance', 8, 'Created requirement: Secret', '2025-12-04 01:03:51'),
(170, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 01:20:20'),
(171, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 01:20:57'),
(172, 9, 'Submit Compliance', 'Compliance', 4, 'Submitted compliance for: Liga Reportasdasdasdsad', '2025-12-04 01:21:05'),
(173, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 01:21:34'),
(174, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 01:24:13'),
(175, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 01:28:53'),
(176, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 01:34:26'),
(177, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 02:45:48'),
(178, 7, 'Create Compliance', 'Compliance', 9, 'Created requirement: Secret', '2025-12-04 02:46:31'),
(179, 7, 'Create Compliance', 'Compliance', 10, 'Created requirement: Secretasdsadsadsa', '2025-12-04 02:46:44'),
(180, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 02:49:43'),
(181, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 02:52:40'),
(182, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 02:59:05'),
(183, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 03:10:21'),
(184, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 03:13:38'),
(185, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 03:18:22'),
(186, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 03:18:34'),
(187, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 03:43:18'),
(188, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 03:48:20'),
(189, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 03:50:38'),
(190, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 03:53:18'),
(191, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 03:54:44'),
(192, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 04:08:50'),
(193, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 04:11:40'),
(194, 5, 'Update User', 'User', 6, 'Updated User: SK0002', '2025-12-04 04:12:00'),
(195, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 04:13:52'),
(196, 5, 'Restore User', 'User', 6, 'Re-elected User: SK0002', '2025-12-04 04:14:03'),
(197, 5, 'Restore User', 'User', 10, 'Re-elected User: SK0005', '2025-12-04 04:14:03'),
(198, 5, 'Update User', 'User', 10, 'Updated User: SK0005', '2025-12-04 04:14:14'),
(199, 5, 'Update User', 'User', 5, 'Updated User: SK0001', '2025-12-04 04:14:36'),
(200, 5, 'Update User', 'User', 5, 'Updated User: SK0001', '2025-12-04 04:15:06'),
(201, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 04:15:15'),
(202, 6, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 04:16:41'),
(203, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 04:18:14'),
(204, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 04:18:29'),
(205, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 04:18:47'),
(206, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 04:26:50'),
(207, 7, 'Add Budget', 'Budget', 0, 'Allocated XDR25,000.00 to Bayanan I', '2025-12-04 04:27:03'),
(208, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 04:27:52'),
(209, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 04:32:42'),
(210, 9, 'Create Project', 'Project', 16, 'Created Project: Sampleasdsda', '2025-12-04 04:32:56'),
(211, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 04:42:52'),
(212, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 04:48:45'),
(213, 9, 'Add Youth', 'YouthMember', 7, 'Added Youth: asdsad asdasdsa', '2025-12-04 04:49:20'),
(214, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 04:51:01'),
(215, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 05:08:02'),
(216, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 05:17:42'),
(217, 9, 'Add Sitio', 'Sitio', 1, 'Added Sitio: Kanluran', '2025-12-04 05:19:39'),
(218, 9, 'Add Sitio', 'Sitio', 2, 'Added Sitio: Hilaga', '2025-12-04 05:19:53'),
(219, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 05:24:43'),
(220, 9, 'Add Sitio', 'Sitio', 3, 'Added Sitio: weh', '2025-12-04 05:25:09'),
(221, 9, 'Add Youth', 'YouthMember', 8, 'Added Youth: Zyris Ortaleza', '2025-12-04 05:25:35'),
(222, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 05:26:41'),
(223, 6, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 05:26:52'),
(224, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 05:28:08'),
(225, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 05:39:27'),
(226, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 05:45:05'),
(227, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 05:52:58'),
(228, 7, 'Approve/Reject Project', 'Project', 16, 'Project Sampleasdsda Status Updated to Approved', '2025-12-04 05:53:08'),
(229, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 05:59:07'),
(230, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 06:08:54'),
(231, 5, 'Add Term', 'KabataanTermPeriod', 8, 'Added New Term: 2031-2034 term', '2025-12-04 06:10:32'),
(232, 5, 'Set Active Term', 'Term', 8, 'Set Active Term to: 2031-2034 term', '2025-12-04 06:10:36'),
(233, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 06:10:57'),
(234, 5, 'Restore User', 'User', 9, 'Re-elected User: SK0004', '2025-12-04 06:11:03'),
(235, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 06:11:14'),
(236, 9, 'Terminate Project', 'Project', 16, 'Terminated Project: Sampleasdsda', '2025-12-04 06:11:31'),
(237, 9, 'Delete/Archive Project', 'Project', 16, 'Archived Project ID 16: Sampleasdsda', '2025-12-04 06:11:58'),
(238, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 06:22:42'),
(239, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 06:25:08'),
(240, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 06:28:47'),
(241, 5, 'Archive User', 'User', 6, 'Resigned/Archived User: SK0002', '2025-12-04 06:28:59'),
(242, 5, 'Restore User', 'User', 6, 'Re-elected User: SK0002', '2025-12-04 06:29:03'),
(243, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 06:29:23'),
(244, 5, 'Archive User', 'User', 7, 'Resigned/Archived User: SK0003', '2025-12-04 06:30:47'),
(245, 5, 'Restore User', 'User', 7, 'Re-elected User: SK0003', '2025-12-04 06:30:50'),
(246, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 06:31:03'),
(247, 7, 'Add Budget', 'Budget', 0, 'Allocated XDR100,000.00 to Bayanan I', '2025-12-04 06:31:19'),
(248, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 06:31:31'),
(249, 9, 'Carry Over Project', 'Project', 16, 'Carried Over Project: Sampleasdsda to Term 2031-2034 term', '2025-12-04 06:31:38'),
(250, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 06:34:07'),
(251, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 06:41:33'),
(252, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 06:46:32'),
(253, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 06:46:50'),
(254, 7, 'Create Compliance', 'Compliance', 11, 'Created requirement: Secretasdsadsadsa', '2025-12-04 06:47:06'),
(255, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 06:53:45'),
(256, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 06:56:11'),
(257, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 06:57:55'),
(258, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 07:00:08'),
(259, 7, 'Create Compliance', 'Compliance', 12, 'Created requirement: Secretasdsadsadsa', '2025-12-04 07:00:20'),
(260, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 07:01:08'),
(261, 9, 'Carry Over Project', 'Project', 16, 'Carried Over Project: Sampleasdsda to Term 2031-2034 term', '2025-12-04 07:01:15'),
(262, 9, 'Terminate Project', 'Project', 16, 'Terminated Project: Sampleasdsda', '2025-12-04 07:01:42'),
(263, 9, 'Terminate Project', 'Project', 16, 'Rejected and Archived Project: Sampleasdsda', '2025-12-04 07:14:30');

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
(5, 'Brent Paulos', 'Bolanos', 'ifdhjbjkfhb@gmail.com', 7, NULL, 0),
(6, 'Brent', 'Dela Cruz', 'ifdhjbjkfhb@gmail.com', 16, 3, 0),
(7, 'Juan', 'Dela Cruz', 'ifdhjbjkfhb@gmail.com', 19, 2, 0),
(9, 'Juan', 'Dela Cruz', 'ifdhjbjkfhb@gmail.com', 5, 3, 0),
(10, 'Jack', 'Jack', 'ralphjohnsales123@gmail.com', 13, 3, 0),
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
  `Sex` enum('Male','Female') NOT NULL,
  `sitio_id` int(11) NOT NULL,
  `Birthday` date NOT NULL,
  `IsArchived` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `youth_member`
--

INSERT INTO `youth_member` (`Member_ID`, `Barangay_ID`, `First_Name`, `Last_Name`, `Age`, `Sex`, `sitio_id`, `Birthday`, `IsArchived`) VALUES
(8, 5, 'Zyris', 'Ortaleza', 20, 'Female', 2, '2005-05-02', 0);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `announcement`
--
ALTER TABLE `announcement`
  ADD PRIMARY KEY (`Announcement_ID`),
  ADD KEY `fk_announcement_user` (`User_ID`);

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
  ADD KEY `fk_budget_barangay` (`Barangay_ID`),
  ADD KEY `fk_budget_term` (`Term_ID`);

--
-- Indexes for table `compliance`
--
ALTER TABLE `compliance`
  ADD PRIMARY KEY (`com_id`),
  ADD KEY `fk_barangay_id` (`Barangay_id`),
  ADD KEY `fk_file_id` (`File_ID`),
  ADD KEY `fk_term_id` (`Term_ID`);

--
-- Indexes for table `federation_fund`
--
ALTER TABLE `federation_fund`
  ADD PRIMARY KEY (`Fund_ID`),
  ADD KEY `Term_ID` (`Term_ID`);

--
-- Indexes for table `file_upload`
--
ALTER TABLE `file_upload`
  ADD PRIMARY KEY (`File_ID`),
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
  ADD KEY `User_ID` (`User_ID`),
  ADD KEY `fk_project_term` (`Term_ID`),
  ADD KEY `fk_file_upload` (`File_ID`);

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
-- Indexes for table `sitio`
--
ALTER TABLE `sitio`
  ADD PRIMARY KEY (`sitio_id`),
  ADD KEY `fk_sitio_barangay` (`Barangay_id`);

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
  ADD KEY `youth_member_ibfk_1` (`Barangay_ID`),
  ADD KEY `fk_sitio_id` (`sitio_id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `announcement`
--
ALTER TABLE `announcement`
  MODIFY `Announcement_ID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `barangay`
--
ALTER TABLE `barangay`
  MODIFY `Barangay_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=63;

--
-- AUTO_INCREMENT for table `budget`
--
ALTER TABLE `budget`
  MODIFY `budget_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT for table `compliance`
--
ALTER TABLE `compliance`
  MODIFY `com_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT for table `federation_fund`
--
ALTER TABLE `federation_fund`
  MODIFY `Fund_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `file_upload`
--
ALTER TABLE `file_upload`
  MODIFY `File_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

--
-- AUTO_INCREMENT for table `kabataan_service_record`
--
ALTER TABLE `kabataan_service_record`
  MODIFY `Record_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=30;

--
-- AUTO_INCREMENT for table `kabataan_term_period`
--
ALTER TABLE `kabataan_term_period`
  MODIFY `Term_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT for table `login`
--
ALTER TABLE `login`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- AUTO_INCREMENT for table `project`
--
ALTER TABLE `project`
  MODIFY `Project_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;

--
-- AUTO_INCREMENT for table `project_allocation`
--
ALTER TABLE `project_allocation`
  MODIFY `Allocation_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `project_log`
--
ALTER TABLE `project_log`
  MODIFY `Log_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `role`
--
ALTER TABLE `role`
  MODIFY `Role_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `sitio`
--
ALTER TABLE `sitio`
  MODIFY `sitio_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `system_log`
--
ALTER TABLE `system_log`
  MODIFY `SysLog_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=264;

--
-- AUTO_INCREMENT for table `user`
--
ALTER TABLE `user`
  MODIFY `User_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- AUTO_INCREMENT for table `youth_member`
--
ALTER TABLE `youth_member`
  MODIFY `Member_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `announcement`
--
ALTER TABLE `announcement`
  ADD CONSTRAINT `fk_announcement_user` FOREIGN KEY (`User_ID`) REFERENCES `user` (`User_ID`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `budget`
--
ALTER TABLE `budget`
  ADD CONSTRAINT `fk_budget_barangay` FOREIGN KEY (`Barangay_ID`) REFERENCES `barangay` (`Barangay_ID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_budget_term` FOREIGN KEY (`Term_ID`) REFERENCES `kabataan_term_period` (`Term_ID`) ON UPDATE CASCADE;

--
-- Constraints for table `compliance`
--
ALTER TABLE `compliance`
  ADD CONSTRAINT `fk_barangay_id` FOREIGN KEY (`Barangay_id`) REFERENCES `barangay` (`Barangay_ID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_file_id` FOREIGN KEY (`File_ID`) REFERENCES `file_upload` (`File_ID`) ON DELETE SET NULL ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_term_id` FOREIGN KEY (`Term_ID`) REFERENCES `kabataan_term_period` (`Term_ID`) ON UPDATE CASCADE;

--
-- Constraints for table `federation_fund`
--
ALTER TABLE `federation_fund`
  ADD CONSTRAINT `federation_fund_ibfk_1` FOREIGN KEY (`Term_ID`) REFERENCES `kabataan_term_period` (`Term_ID`);

--
-- Constraints for table `file_upload`
--
ALTER TABLE `file_upload`
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
  ADD CONSTRAINT `fk_file_upload` FOREIGN KEY (`File_ID`) REFERENCES `file_upload` (`File_ID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_project_term` FOREIGN KEY (`Term_ID`) REFERENCES `kabataan_term_period` (`Term_ID`) ON UPDATE CASCADE,
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
-- Constraints for table `sitio`
--
ALTER TABLE `sitio`
  ADD CONSTRAINT `fk_sitio_barangay` FOREIGN KEY (`Barangay_id`) REFERENCES `barangay` (`Barangay_ID`) ON DELETE CASCADE ON UPDATE CASCADE;

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
  ADD CONSTRAINT `barangay_id` FOREIGN KEY (`Barangay_ID`) REFERENCES `barangay` (`Barangay_ID`) ON DELETE CASCADE,
  ADD CONSTRAINT `fk_sitio_id` FOREIGN KEY (`sitio_id`) REFERENCES `sitio` (`sitio_id`) ON DELETE CASCADE ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

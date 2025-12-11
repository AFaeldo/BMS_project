-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Dec 11, 2025 at 12:18 AM
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

--
-- Dumping data for table `announcement`
--

INSERT INTO `announcement` (`Announcement_ID`, `User_ID`, `Title`, `Message`, `IsActive`, `Date_Created`) VALUES
(1, 7, 'Monthly Federation Meeting', 'All SK Chairpersons are required to attend the meeting at the City Hall this Friday, 2 PM.', 0, '2024-03-10 08:00:00'),
(2, 7, 'Linggo ng Kabataan 2024', 'Prepare your line-up for the upcoming sportsfest!', 0, '2024-04-01 09:00:00'),
(3, 6, 'Free Vaccination Drive', 'Happening at Barangay Canubing II covered court.', 1, '2024-02-15 10:00:00');

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
(62, 'Wawa'),
(63, 'Salong');

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
(1, 16, 500000.00, 150000.00, 350000.00, 1),
(2, 5, 450000.00, 350650.00, 99350.00, 1),
(3, 13, 600000.00, 200000.00, 400000.00, 1),
(4, 58, 300000.00, 0.00, 300000.00, 1),
(5, 63, 500000.00, 250000.00, 250000.00, 1);

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
  `IsArchived` tinyint(1) NOT NULL,
  `TemplateFile_ID` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `compliance`
--

INSERT INTO `compliance` (`com_id`, `Barangay_id`, `File_ID`, `Title`, `Type`, `Status`, `due_date`, `Date_Submitted`, `Term_ID`, `IsArchived`, `TemplateFile_ID`) VALUES
(7, 5, NULL, 'Completion Report: Sampleasd', 'Project Completion Report', 'Approved', '2025-12-23', '2025-12-08 15:16:21', 1, 0, NULL),
(8, 5, NULL, 'Liga Report', 'Report', 'Completed', '2025-12-09', '2025-12-08 16:48:08', 1, 0, 16),
(9, 5, NULL, 'asdasdasdasd', 'Financial Statement', 'Completed', '2025-12-09', '2025-12-08 17:24:20', 1, 0, 21),
(10, 5, NULL, 'asdasdasdasd', 'Report', 'Completed', '2025-12-10', '2025-12-08 17:35:27', 1, 0, 24),
(11, 5, NULL, 'Monthly Federation Meeting', 'ASDASDASDSA', 'Not Submitted', '2025-12-01', NULL, 1, 0, 27),
(12, 5, NULL, 'Completion Report: SampleasdsdaadssadsaefwFEWDA43', 'Project Completion Report', 'Not Submitted', '2025-12-25', NULL, 1, 0, NULL),
(13, 4, NULL, 'Liga Reportasd', 'Ano ba', 'Completed', '2025-12-01', '2025-12-11 06:34:18', 1, 0, 43);

-- --------------------------------------------------------

--
-- Table structure for table `compliance_document`
--

CREATE TABLE `compliance_document` (
  `Document_ID` int(11) NOT NULL,
  `Compliance_ID` int(11) NOT NULL,
  `File_ID` int(11) NOT NULL,
  `Status` varchar(50) DEFAULT 'Pending',
  `Remarks` longtext DEFAULT NULL,
  `Date_Submitted` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `compliance_document`
--

INSERT INTO `compliance_document` (`Document_ID`, `Compliance_ID`, `File_ID`, `Status`, `Remarks`, `Date_Submitted`) VALUES
(4, 7, 14, 'Approved', NULL, '2025-12-08 15:16:21'),
(5, 7, 15, 'Approved', NULL, '2025-12-08 15:16:21'),
(6, 8, 17, 'Approved', NULL, '2025-12-08 16:38:28'),
(7, 8, 18, 'Rejected', 'Mali Spelling', '2025-12-08 16:38:28'),
(8, 8, 19, 'Approved', NULL, '2025-12-08 16:39:33'),
(9, 8, 20, 'Approved', NULL, '2025-12-08 16:48:08'),
(10, 9, 22, 'Approved', NULL, '2025-12-08 17:24:20'),
(11, 9, 23, 'Approved', NULL, '2025-12-08 17:24:20'),
(12, 10, 25, 'Approved', NULL, '2025-12-08 17:35:27'),
(13, 10, 26, 'Approved', NULL, '2025-12-08 17:35:27'),
(14, 13, 44, 'Approved', NULL, '2025-12-11 06:34:18');

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
(1, 1, 10000000.00, 3000000.00, '2024-01-01 00:00:00');

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
(1, 'project_proposal_liga.pdf', 'uploads/project_proposal_liga.pdf', 6, '2025-12-04 07:50:10'),
(2, 'budget_plan_2024.xlsx', 'uploads/budget_plan_2024.xlsx', 9, '2025-12-04 07:50:10'),
(3, 'cleanup_drive_sched.jpg', 'uploads/cleanup_drive_sched.jpg', 10, '2025-12-04 07:50:10'),
(4, 'ab_yip_2024.pdf', 'uploads/ab_yip_2024.pdf', 11, '2025-12-04 07:50:10'),
(5, 'resolution_no_1.pdf', 'uploads/resolution_no_1.pdf', 7, '2025-12-04 07:50:10'),
(6, 'seminar_proposal.pdf', 'uploads/seminar_proposal.pdf', 6, '2025-12-04 07:50:10'),
(7, 'Sampledasf ', '/UploadedFiles/Sampledasf _20251208120938_60ba56.pdf', 9, '2025-12-08 12:09:38'),
(8, 'Sampledasf ', '/UploadedFiles/Sampledasf _20251208133121_00059d.pdf', 9, '2025-12-08 13:31:21'),
(10, 'Exercise 9.pdf', '/UploadedFiles/Submissions/5_20251208143837_1df1f4.pdf', 9, '2025-12-08 14:38:37'),
(11, 'LabExercise5.pdf', '/UploadedFiles/Submissions/5_20251208143837_4d0439.pdf', 9, '2025-12-08 14:38:37'),
(12, 'Elective-Presentation.pdf', '/UploadedFiles/Submissions/5_20251208143837_32b20d.pdf', 9, '2025-12-08 14:38:37'),
(13, 'Sampledasf ', '/UploadedFiles/Sampledasf _20251208145004_eb5b19.pdf', 9, '2025-12-08 14:50:04'),
(14, 'LabExercise5.pdf', '/UploadedFiles/Submissions/7_20251208151621_98feb6.pdf', 9, '2025-12-08 15:16:21'),
(15, 'Exercise 8.pdf', '/UploadedFiles/Submissions/7_20251208151621_10b7d5.pdf', 9, '2025-12-08 15:16:21'),
(16, 'Exer9.docx', '/UploadedFiles/Templates/Template_20251208163313_08cb17.docx', 7, '2025-12-08 16:33:13'),
(17, 'Exercise 9 (1).pdf', '/UploadedFiles/Submissions/8_20251208163828_efbfd5.pdf', 9, '2025-12-08 16:38:28'),
(18, 'LabExercise5.pdf', '/UploadedFiles/Submissions/8_20251208163828_1ea84e.pdf', 9, '2025-12-08 16:38:28'),
(19, 'Exercise 9.pdf', '/UploadedFiles/Submissions/8_20251208163933_d393b5.pdf', 9, '2025-12-08 16:39:33'),
(20, 'ITP108_A_PREFINAL.pdf', '/UploadedFiles/Submissions/8_20251208164808_926a74.pdf', 9, '2025-12-08 16:48:08'),
(21, 'Exer9.docx', '/UploadedFiles/Templates/Template_20251208172313_0e2d29.docx', 7, '2025-12-08 17:23:13'),
(22, 'Elective-Presentation.pdf', '/UploadedFiles/Submissions/9_20251208172420_8469f4.pdf', 9, '2025-12-08 17:24:20'),
(23, 'APBP.pdf', '/UploadedFiles/Submissions/9_20251208172420_a39b86.pdf', 9, '2025-12-08 17:24:20'),
(24, 'Merging Data with Power Query-1.docx', '/UploadedFiles/Templates/Template_20251208173506_7dec05.docx', 7, '2025-12-08 17:35:06'),
(25, 'Exercise 9.pdf', '/UploadedFiles/Submissions/10_20251208173527_6abcc7.pdf', 9, '2025-12-08 17:35:27'),
(26, 'Exercise 9 (1).pdf', '/UploadedFiles/Submissions/10_20251208173527_35409e.pdf', 9, '2025-12-08 17:35:27'),
(27, 'Exer9.docx', '/UploadedFiles/Templates/Template_20251208180453_06150f.docx', 7, '2025-12-08 18:04:53'),
(28, 'Sampledasf ', '/UploadedFiles/Sampledasf _20251208182222_d9f69b.pdf', 9, '2025-12-08 18:22:22'),
(29, 'ITP108_A_PREFINAL.pdf', '/UploadedFiles/ITP108_A_PREFINAL_20251208184653_6f2f18.pdf', 9, '2025-12-08 18:46:53'),
(30, 'APBP.pdf', '/UploadedFiles/APBP_20251208184653_14bc1c.pdf', 9, '2025-12-08 18:46:53'),
(31, 'Elective-Presentation.pdf', '/UploadedFiles/Elective-Presentation_20251208184653_ceea01.pdf', 9, '2025-12-08 18:46:53'),
(32, 'Exercise 9 (1).pdf', '/UploadedFiles/Exercise 9 (1)_20251208184653_01ceca.pdf', 9, '2025-12-08 18:46:53'),
(33, 'Exercise 9.pdf', '/UploadedFiles/Exercise 9_20251208184653_969696.pdf', 9, '2025-12-08 18:46:53'),
(34, 'Exercise 8.pdf', '/UploadedFiles/Exercise 8_20251208184653_ec7dcf.pdf', 9, '2025-12-08 18:46:53'),
(35, 'LabExercise5.pdf', '/UploadedFiles/LabExercise5_20251208192525_5be3b7.pdf', 9, '2025-12-08 19:25:25'),
(36, 'Elective-Presentation.pdf', '/UploadedFiles/Elective-Presentation_20251208192525_81bb16.pdf', 9, '2025-12-08 19:25:25'),
(37, 'Exercise 8.pdf', '/UploadedFiles/Exercise 8_20251210110436_9037bc.pdf', 9, '2025-12-10 11:04:36'),
(38, 'LabExercise5.pdf', '/UploadedFiles/LabExercise5_20251210110437_a7d305.pdf', 9, '2025-12-10 11:04:37'),
(39, 'Exercise 9.pdf', '/UploadedFiles/Exercise 9_20251210110509_bfafe9.pdf', 9, '2025-12-10 11:05:09'),
(40, 'Exercise 9.pdf', '/UploadedFiles/Exercise 9_20251210111735_1a226c.pdf', 9, '2025-12-10 11:17:35'),
(41, 'Exercise 8.pdf', '/UploadedFiles/Exercise 8_20251210223737_8c6d5c.pdf', 17, '2025-12-10 22:37:37'),
(42, 'Elective-Presentation.pdf', '/UploadedFiles/Elective-Presentation_20251211060027_e57927.pdf', 9, '2025-12-11 06:00:27'),
(43, 'SIT-TECH-TRAINING-Capstone-Fundamentals.docx', '/UploadedFiles/Templates/Template_20251211062351_cbbc55.docx', 7, '2025-12-11 06:23:51'),
(44, 'Elective-Presentation.pdf', '/UploadedFiles/Submissions/13_20251211063418_a1d1cb.pdf', 17, '2025-12-11 06:34:18');

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
(1, 6, 1, 3, 'Active', NULL),
(2, 7, 1, 2, 'Active', NULL),
(3, 9, 1, 3, 'Active', NULL),
(4, 10, 1, 3, 'Active', NULL),
(5, 11, 1, 3, 'Active', NULL),
(9, 15, 1, 3, 'Active', NULL),
(10, 16, 1, 3, 'Active', NULL),
(11, 17, 1, 3, 'Active', NULL);

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
(1, 'SK Term 2023-2025', '2023-11-01', '2025-11-01', 1),
(2, 'SK Term 2018-2022', '2018-06-30', '2022-12-31', 0),
(3, '2010 - 2013', '2010-01-01', '2013-01-04', 0);

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
(11, 'SK0006', 'AQAAAAIAAYagAAAAELRiCqGnXF29qpS1qBMcAzJDh/nkgg06TdQcYJtAndvRuB4/znVJe4kenwcORtJnpw==', 3, 11),
(15, 'SK0007', 'AQAAAAIAAYagAAAAEHyZKI+LODRu1DY8DrVUakuQTezcIN6tMUBiPNJGf1OZkyPa+x3G80M7XEUsE2HfUA==', 3, 15),
(16, 'SK0008', 'AQAAAAIAAYagAAAAEMjgzC9Zp/YcyUsfI/iFJeeOsbPHjtDct1J24li7R8jWZ4rCa9blGt4LVuDpJbrmzA==', 3, 16),
(17, 'SK0009', 'AQAAAAIAAYagAAAAEJ1N/md+NzH59uurJzMe1AvWitLS968z2lnysS4Lfe5nezNuPb9eYweFT+hSjQYZ3Q==', 3, 17);

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
  `Project_Status` enum('Pending','Approved','Rejected','Completed','Ongoing') NOT NULL DEFAULT 'Pending',
  `Start_Date` date NOT NULL,
  `End_Date` date NOT NULL,
  `IsArchived` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `project`
--

INSERT INTO `project` (`Project_ID`, `User_ID`, `Term_ID`, `File_ID`, `Project_Title`, `Project_Description`, `Estimated_Cost`, `Date_Submitted`, `Project_Status`, `Start_Date`, `End_Date`, `IsArchived`) VALUES
(1, 6, 1, 1, 'Inter-Barangay Basketball League', 'Annual sports festival for the youth of Canubing II.', 50000.00, '2024-01-15', 'Approved', '2024-03-01', '2024-04-01', 0),
(2, 9, 1, 2, 'Community Garden Initiative', 'Planting vegetables in vacant lots in Bayanan I.', 15000.00, '2024-02-10', 'Pending', '2024-05-01', '2024-05-05', 1),
(3, 10, 1, 3, 'Coastal Clean-up Drive', 'Cleaning the shores of Camansihan.', 5000.00, '2024-01-20', 'Completed', '2024-02-01', '2024-02-01', 0),
(4, 6, 1, 6, 'Youth Leadership Seminar', 'Training regarding governance and leadership.', 25000.00, '2024-03-05', 'Approved', '2024-04-10', '2024-04-12', 0),
(5, 11, 1, 1, 'Battle of the Bands', 'Music festival for local talents in Suqui.', 30000.00, '2024-06-01', 'Rejected', '2024-07-01', '2024-07-02', 0),
(6, 9, 1, 7, 'Community Garden Initiatives', 'ZXdasdadsad', 0.00, '2025-12-08', 'Completed', '2024-02-08', '2025-08-08', 1),
(7, 9, 1, 8, 'Sample', 'sample', 0.00, '2025-12-08', 'Completed', '2025-12-08', '2025-12-09', 1),
(8, 9, 1, 13, 'Sampleasd', 'asddsasda', 0.00, '2025-12-08', 'Completed', '2025-12-08', '2025-12-09', 0),
(9, 9, 1, 28, 'Sampleasdsda', 'dasdsasda', 0.00, '2025-12-08', 'Rejected', '2025-12-10', '2025-12-14', 0),
(10, 9, 1, 29, 'SampleasdsdaadssadsaefwFEWDA43', 'qewqweqwweqwe', 0.00, '2025-12-08', 'Completed', '2025-12-15', '2025-12-27', 0),
(14, 9, 1, 40, 'Community Garden Initiativeasdasdsadfadsasddfsadfdwsfasdsadsadas', 'asddasasdsadasddas', 0.00, '2025-12-10', 'Approved', '2026-01-07', '2026-01-08', 0),
(15, 17, 1, 41, 'lin', 'linis', 0.00, '2025-12-10', 'Approved', '2025-12-11', '2025-12-27', 0),
(16, 9, 1, 42, 'Community Garden Initiativeasdasdsadsad123213', '123132131313', 0.00, '2025-12-11', 'Pending', '2026-02-01', '2026-02-02', 0);

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
(1, 1, 1, 50000.00),
(2, 3, 3, 5000.00),
(3, 1, 4, 25000.00),
(4, 2, 6, 50.00),
(5, 2, 7, 50.00),
(6, 2, 8, 500.00),
(7, 2, 9, 500.00),
(8, 2, 10, 50.00),
(12, 2, 14, 300000.00),
(13, 5, 15, 250000.00),
(14, 2, 16, 50000.00);

-- --------------------------------------------------------

--
-- Table structure for table `project_document`
--

CREATE TABLE `project_document` (
  `Document_ID` int(11) NOT NULL,
  `Project_ID` int(11) NOT NULL,
  `File_ID` int(11) NOT NULL,
  `Date_Added` datetime(6) NOT NULL,
  `Description` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `project_document`
--

INSERT INTO `project_document` (`Document_ID`, `Project_ID`, `File_ID`, `Date_Added`, `Description`) VALUES
(1, 10, 29, '2025-12-08 18:46:53.833469', NULL),
(2, 10, 30, '2025-12-08 18:46:53.841747', NULL),
(3, 10, 31, '2025-12-08 18:46:53.841799', NULL),
(4, 10, 32, '2025-12-08 18:46:53.841819', NULL),
(5, 10, 33, '2025-12-08 18:46:53.841839', NULL),
(6, 10, 34, '2025-12-08 18:46:53.841858', NULL),
(12, 14, 40, '2025-12-10 11:17:36.018219', NULL),
(13, 15, 41, '2025-12-10 22:37:37.517194', NULL),
(14, 16, 42, '2025-12-11 06:00:27.873642', NULL);

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
(1, 1, 6, 'Pending', '2024-01-15 08:00:00', 'Submitted for review'),
(2, 1, 7, 'Approved', '2024-01-20 10:00:00', 'Approved by Fed Pres'),
(3, 3, 10, 'Pending', '2024-01-20 09:00:00', 'Submitted proposal'),
(4, 3, 10, 'Completed', '2024-02-02 17:00:00', 'Event finished successfully'),
(5, 5, 11, 'Pending', '2024-06-01 08:30:00', 'Requesting funds'),
(6, 5, 7, 'Rejected', '2024-06-05 11:00:00', 'Budget constraints'),
(7, 6, 9, 'Pending', '2025-12-08 12:09:38', 'Project created and submitted for approval.'),
(8, 7, 9, 'Pending', '2025-12-08 13:31:21', 'Project created and submitted for approval.'),
(9, 6, 9, 'Approved', '2025-12-08 13:31:50', 'Project status updated to Approved by Federation President'),
(10, 7, 9, 'Approved', '2025-12-08 13:34:24', 'Project status updated to Approved by Federation President'),
(11, 8, 9, 'Pending', '2025-12-08 14:50:04', 'Project created and submitted for approval.'),
(12, 8, 9, 'Approved', '2025-12-08 14:51:56', 'Project status updated to Approved by Federation President'),
(13, 9, 9, 'Pending', '2025-12-08 18:22:22', 'Project created and submitted for approval.'),
(14, 10, 9, 'Pending', '2025-12-08 18:46:53', 'Project created and submitted for approval.'),
(18, 14, 9, 'Pending', '2025-12-10 11:17:36', 'Project created and submitted for approval.'),
(19, 14, 9, 'Pending', '2025-12-10 22:19:20', 'Project status updated to Pending by Federation President'),
(20, 14, 9, 'Pending', '2025-12-10 22:30:47', 'Project status updated to Pending by Federation President'),
(21, 14, 9, 'Pending', '2025-12-10 22:31:07', 'Project status updated to Pending by Federation President'),
(22, 10, 9, 'Approved', '2025-12-10 22:33:41', 'Project status updated to Approved by Federation President'),
(23, 15, 17, 'Pending', '2025-12-10 22:37:37', 'Project created and submitted for approval.'),
(24, 14, 9, 'Approved', '2025-12-11 05:52:20', 'Project status updated to Approved by Federation President'),
(25, 16, 9, 'Pending', '2025-12-11 06:00:27', 'Project created and submitted for approval.'),
(26, 9, 9, 'Rejected', '2025-12-11 06:10:22', 'Trip ko lang'),
(27, 9, 9, 'Rejected', '2025-12-11 06:11:20', 'Rejection Acknowledged'),
(28, 15, 17, 'Approved', '2025-12-11 06:23:11', 'Project status updated to Approved by Federation President');

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
(1, 5, 'Purok 1 - Mangga'),
(2, 5, 'Purok 2 - Santol'),
(3, 13, 'Zone 1'),
(4, 13, 'Zone 2'),
(5, 16, 'Sitio Kawayan'),
(6, 19, 'Lower Gulod'),
(7, 19, 'Upper Gulod'),
(8, 58, 'Riverside'),
(9, 58, 'Crossing');

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
(1, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 07:50:55'),
(2, 5, 'Edit Announcement', 'Announcement', 1, 'Updated Announcement: Monthly Federation Meeting', '2025-12-04 07:51:20'),
(3, 5, 'Edit Announcement', 'Announcement', 3, 'Updated Announcement: Free Vaccination Drive', '2025-12-04 07:51:24'),
(4, 5, 'Edit Announcement', 'Announcement', 1, 'Updated Announcement: Monthly Federation Meeting', '2025-12-04 07:51:27'),
(5, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 07:52:07'),
(6, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 07:53:02'),
(7, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-04 07:55:05'),
(8, 5, 'Set Active Term', 'Term', 2, 'Set Active Term to: SK Term 2018-2022', '2025-12-04 07:55:13'),
(9, 5, 'Set Active Term', 'Term', 1, 'Set Active Term to: SK Term 2023-2025', '2025-12-04 07:55:17'),
(10, 5, 'Add Term', 'KabataanTermPeriod', 3, 'Added New Term: 2010 - 2013', '2025-12-04 07:55:58'),
(11, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-05 06:48:53'),
(12, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 10:59:57'),
(13, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 12:04:41'),
(14, 9, 'Create Project', 'Project', 6, 'Created Project: Community Garden Initiatives', '2025-12-08 12:09:38'),
(15, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 12:33:02'),
(16, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 12:47:36'),
(17, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 12:53:37'),
(18, 5, 'Create User', 'User', 12, 'Created User: SK0007', '2025-12-08 12:54:04'),
(19, 5, 'Update User', 'User', 6, 'Updated User: SK0002', '2025-12-08 13:07:32'),
(20, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 13:12:49'),
(21, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 13:30:44'),
(22, 9, 'Create Project', 'Project', 7, 'Created Project: Sample', '2025-12-08 13:31:21'),
(23, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 13:31:31'),
(24, 7, 'Approve/Reject Project', 'Project', 6, 'Project Community Garden Initiatives Status Updated to Approved', '2025-12-08 13:31:50'),
(25, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 13:32:02'),
(26, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 13:32:15'),
(27, 9, 'Update Project', 'Project', 6, 'Updated Project Community Garden Initiatives status to Completed', '2025-12-08 13:32:33'),
(28, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 13:33:17'),
(29, 7, 'Approve/Reject Project', 'Project', 7, 'Project Sample Status Updated to Approved', '2025-12-08 13:34:24'),
(30, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 13:34:37'),
(31, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 13:34:55'),
(32, 9, 'Update Project', 'Project', 7, 'Updated Project Sample status to Ongoing', '2025-12-08 13:36:00'),
(33, 9, 'Update Project', 'Project', 7, 'Updated Project Sample status to Completed', '2025-12-08 13:37:59'),
(34, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 13:39:02'),
(35, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 13:41:51'),
(36, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 14:18:09'),
(37, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 14:18:09'),
(38, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 14:38:16'),
(39, 9, 'Submit Compliance', 'Compliance', 5, 'Submitted 3 documents for: Completion Report: Community Garden Initiatives', '2025-12-08 14:38:37'),
(40, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 14:39:01'),
(41, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 14:39:23'),
(42, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 14:43:26'),
(43, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 14:46:25'),
(44, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 14:48:46'),
(45, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 14:49:13'),
(46, 9, 'Delete/Archive Project', 'Project', 6, 'Archived Project ID 6: Community Garden Initiatives', '2025-12-08 14:49:18'),
(47, 9, 'Delete/Archive Project', 'Project', 7, 'Archived Project ID 7: Sample', '2025-12-08 14:49:19'),
(48, 9, 'Delete/Archive Project', 'Project', 2, 'Archived Project ID 2: Community Garden Initiative', '2025-12-08 14:49:21'),
(49, 9, 'Create Project', 'Project', 8, 'Created Project: Sampleasd', '2025-12-08 14:50:04'),
(50, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 14:50:13'),
(51, 7, 'Approve/Reject Project', 'Project', 8, 'Project Sampleasd Status Updated to Approved', '2025-12-08 14:51:56'),
(52, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 14:52:07'),
(53, 9, 'Update Project', 'Project', 8, 'Updated Project Sampleasd status to Completed', '2025-12-08 14:52:18'),
(54, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 14:55:12'),
(55, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 15:16:05'),
(56, 9, 'Submit Compliance', 'Compliance', 7, 'Submitted 2 documents for: Completion Report: Sampleasd', '2025-12-08 15:16:21'),
(57, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 15:16:41'),
(58, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 15:17:10'),
(59, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 15:17:31'),
(60, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 15:24:11'),
(61, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 15:24:22'),
(62, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 15:24:58'),
(63, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 15:25:10'),
(64, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 15:51:20'),
(65, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 15:55:24'),
(66, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 16:32:25'),
(67, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 16:32:45'),
(68, 7, 'Create Compliance', 'Compliance', 8, 'Created requirement: Liga Report', '2025-12-08 16:33:13'),
(69, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 16:33:24'),
(70, 9, 'Submit Compliance', 'Compliance', 8, 'Submitted 2 documents for: Liga Report', '2025-12-08 16:38:28'),
(71, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 16:38:40'),
(72, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 16:39:10'),
(73, 9, 'Submit Compliance', 'Compliance', 8, 'Submitted 1 documents for: Liga Report', '2025-12-08 16:39:33'),
(74, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 16:39:42'),
(75, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 16:40:16'),
(76, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 16:42:41'),
(77, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 16:47:58'),
(78, 9, 'Submit Compliance', 'Compliance', 8, 'Submitted 1 documents for: Liga Report', '2025-12-08 16:48:08'),
(79, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 16:48:17'),
(80, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 16:48:51'),
(81, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 16:49:07'),
(82, 7, 'Create Compliance', 'Compliance', 9, 'Created requirement: asdasdasdasd', '2025-12-08 17:23:13'),
(83, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 17:23:27'),
(84, 9, 'Submit Compliance', 'Compliance', 9, 'Submitted 2 documents for: asdasdasdasd', '2025-12-08 17:24:20'),
(85, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 17:24:29'),
(86, 7, 'Create Compliance', 'Compliance', 10, 'Created requirement: asdasdasdasd', '2025-12-08 17:35:06'),
(87, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 17:35:13'),
(88, 9, 'Submit Compliance', 'Compliance', 10, 'Submitted 2 documents for: asdasdasdasd', '2025-12-08 17:35:27'),
(89, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 17:35:35'),
(90, 7, 'Create Compliance', 'Compliance', 11, 'Created requirement: Monthly Federation Meeting', '2025-12-08 18:04:53'),
(91, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 18:11:53'),
(92, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 18:21:43'),
(93, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 18:21:51'),
(94, 9, 'Create Project', 'Project', 9, 'Created Project: Sampleasdsda', '2025-12-08 18:22:22'),
(95, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 18:25:41'),
(96, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 18:29:14'),
(97, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 18:37:06'),
(98, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 18:44:56'),
(99, 9, 'Create Project', 'Project', 10, 'Created Project: SampleasdsdaadssadsaefwFEWDA43 with 6 documents', '2025-12-08 18:46:53'),
(100, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 18:47:11'),
(101, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 19:08:41'),
(102, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 19:22:18'),
(103, 9, 'Create Project', 'Project', 11, 'Created Project: Community Garden Initiativesdadassdaasd with 2 documents', '2025-12-08 19:25:25'),
(104, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 19:25:36'),
(105, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 19:34:38'),
(106, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-08 19:37:59'),
(107, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-10 11:02:56'),
(108, 9, 'Create Project', 'Project', 12, 'Created Project: Community Garden Initiativeasdasdsad with 2 documents', '2025-12-10 11:04:37'),
(109, 9, 'Create Project', 'Project', 13, 'Created Project: Community Garden Initiativeasdasdsadfadsasddfsadfdwsf with 1 documents', '2025-12-10 11:05:09'),
(110, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-10 11:15:06'),
(111, 9, 'Create Project', 'Project', 14, 'Created Project: Community Garden Initiativeasdasdsadfadsasddfsadfdwsfasdsadsadas with 1 documents', '2025-12-10 11:17:36'),
(112, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-10 13:04:13'),
(113, 5, 'Edit Announcement', 'Announcement', 2, 'Updated Announcement: Linggo ng Kabataan 2024', '2025-12-10 13:04:26'),
(114, 5, 'Create User', 'User', 13, 'Created User: SK0008', '2025-12-10 14:05:38'),
(115, 5, 'Email Error', 'User', 13, 'Failed to send verification email to ifdhjbjkfhb@gmail.com: Failure sending mail.', '2025-12-10 14:05:39'),
(116, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-10 14:20:25'),
(117, 5, 'Create User', 'User', 14, 'Created User: SK0009', '2025-12-10 14:22:14'),
(118, 5, 'Email Error', 'User', 14, 'Failed to send verification email to dsaddasd@bms.com: Mailbox unavailable. The server response was: 5.7.1 Security check pending. Mailtrap is checking your domain credibility, it usually takes one business day.', '2025-12-10 14:22:18'),
(119, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-10 19:56:03'),
(120, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-10 20:29:46'),
(121, 5, 'Create User', 'User', 15, 'Created User: SK0007', '2025-12-10 20:30:41'),
(122, 5, 'Create User', 'User', 16, 'Created User: SK0008', '2025-12-10 20:34:04'),
(123, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-10 21:11:03'),
(124, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-10 22:05:55'),
(125, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-10 22:07:23'),
(126, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-10 22:08:21'),
(127, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-10 22:08:48'),
(128, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-10 22:14:42'),
(129, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-10 22:16:05'),
(130, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-10 22:17:12'),
(131, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-10 22:17:22'),
(132, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-10 22:17:43'),
(133, 7, 'Approve/Reject Project', 'Project', 14, 'Project Community Garden Initiativeasdasdsadfadsasddfsadfdwsfasdsadsadas Status Updated to Pending', '2025-12-10 22:19:20'),
(134, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-10 22:19:32'),
(135, 5, 'Add Barangay', 'Barangay', 63, 'Added Barangay: Salong', '2025-12-10 22:22:15'),
(136, 5, 'Create User', 'User', 17, 'Created User: SK0009', '2025-12-10 22:22:42'),
(137, 17, 'Login', NULL, NULL, 'User Logged In', '2025-12-10 22:23:44'),
(138, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-10 22:25:55'),
(139, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-10 22:26:07'),
(140, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-10 22:29:09'),
(141, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-10 22:30:31'),
(142, 7, 'Approve/Reject Project', 'Project', 14, 'Project Community Garden Initiativeasdasdsadfadsasddfsadfdwsfasdsadsadas Status Updated to Pending', '2025-12-10 22:30:47'),
(143, 7, 'Approve/Reject Project', 'Project', 14, 'Project Community Garden Initiativeasdasdsadfadsasddfsadfdwsfasdsadsadas Status Updated to Pending', '2025-12-10 22:31:07'),
(144, 7, 'Approve/Reject Project', 'Project', 10, 'Project SampleasdsdaadssadsaefwFEWDA43 Status Updated to Approved', '2025-12-10 22:33:41'),
(145, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-10 22:33:50'),
(146, 9, 'Update Project', 'Project', 10, 'Updated Project SampleasdsdaadssadsaefwFEWDA43 status to Ongoing', '2025-12-10 22:34:02'),
(147, 9, 'Update Project', 'Project', 10, 'Updated Project SampleasdsdaadssadsaefwFEWDA43 status to Completed', '2025-12-10 22:34:13'),
(148, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-10 22:34:42'),
(149, 7, 'Add Budget', 'Budget', 0, 'Allocated ₱500,000.00 to Salong', '2025-12-10 22:35:24'),
(150, 17, 'Login', NULL, NULL, 'User Logged In', '2025-12-10 22:35:40'),
(151, 17, 'Create Project', 'Project', 15, 'Created Project: lin with 1 documents', '2025-12-10 22:37:37'),
(152, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-10 22:42:00'),
(153, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 05:34:30'),
(154, 5, 'Edit Announcement', 'Announcement', 3, 'Updated Announcement: Free Vaccination Drive', '2025-12-11 05:35:12'),
(155, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 05:37:12'),
(156, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 05:42:27'),
(157, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 05:47:09'),
(158, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 05:51:22'),
(159, 7, 'Approve/Reject Project', 'Project', 14, 'Project Community Garden Initiativeasdasdsadfadsasddfsadfdwsfasdsadsadas Status Updated to Approved', '2025-12-11 05:52:20'),
(160, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 05:53:06'),
(161, 9, 'Create Project', 'Project', 16, 'Created Project: Community Garden Initiativeasdasdsadsad123213 with 1 documents', '2025-12-11 06:00:27'),
(162, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 06:01:23'),
(163, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 06:10:03'),
(164, 7, 'Approve/Reject Project', 'Project', 9, 'Project Sampleasdsda Status Updated to Rejected', '2025-12-11 06:10:22'),
(165, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 06:10:50'),
(166, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 06:17:10'),
(167, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 06:17:10'),
(168, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 06:22:27'),
(169, 7, 'Approve/Reject Project', 'Project', 15, 'Project lin Status Updated to Approved', '2025-12-11 06:23:11'),
(170, 7, 'Create Compliance', 'Compliance', 13, 'Created requirement: Liga Reportasd', '2025-12-11 06:23:51'),
(171, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 06:28:58'),
(172, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 06:29:20'),
(173, 5, 'Update User', 'User', 17, 'Updated User: SK0009', '2025-12-11 06:29:36'),
(174, 17, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 06:29:52'),
(175, 17, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 06:33:44'),
(176, 17, 'Submit Compliance', 'Compliance', 13, 'Submitted 1 documents for: Liga Reportasd', '2025-12-11 06:34:18'),
(177, 17, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 06:39:39'),
(178, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 06:40:03'),
(179, 17, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 06:40:34'),
(180, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 06:47:43'),
(181, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 06:48:52'),
(182, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 06:54:02'),
(183, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 07:10:02'),
(184, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 07:14:19'),
(185, 9, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 07:14:28'),
(186, 7, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 07:16:46'),
(187, 5, 'Login', NULL, NULL, 'User Logged In', '2025-12-11 07:17:13');

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
  `IsArchived` tinyint(1) NOT NULL,
  `IsEmailVerified` tinyint(1) NOT NULL DEFAULT 0,
  `VerificationToken` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `user`
--

INSERT INTO `user` (`User_ID`, `First_Name`, `Last_Name`, `Email`, `Barangay_ID`, `Role_ID`, `IsArchived`, `IsEmailVerified`, `VerificationToken`) VALUES
(5, 'Brent Paulos', 'Bolanos', 'ifdhjbjkfhb@gmail.com', 7, 1, 0, 1, NULL),
(6, 'Brent', 'Dela Cruz', 'ifdhjbjkfhb@gmail.com', 16, 3, 0, 1, NULL),
(7, 'Juan', 'Dela Cruz', 'ifdhjbjkfhb@gmail.com', 19, 2, 0, 1, NULL),
(9, 'Juan', 'Dela Cruz', 'ifdhjbjkfhb@gmail.com', 5, 3, 0, 1, NULL),
(10, 'Jack', 'Jack', 'ralphjohnsales123@gmail.com', 13, 3, 0, 1, NULL),
(11, 'Zyris', 'Ortaleza', 'sklnjdb@gmail.com', 58, 3, 0, 1, NULL),
(15, 'Brent Paulosddfdf', 'Bolanos', 'bolanos.brentpaulo07@gmail.com', 16, 3, 0, 1, NULL),
(16, 'Brent Paulosddfdf', 'Brent Paulosddfdfasdsadsad', 'dsaddasd@bms.com', 19, 3, 0, 1, NULL),
(17, 'Kurt', 'Tugas', 'itzkurt@gmail.com', 4, 3, 0, 1, NULL);

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
(1, 5, 'Michael', 'Jordan', 19, 'Male', 1, '2005-02-17', 0),
(2, 5, 'Kobe', 'Bryant', 22, 'Male', 2, '2002-08-23', 0),
(3, 13, 'Serena', 'Williams', 21, 'Female', 3, '2003-09-26', 0),
(4, 13, 'Maria', 'Sharapova', 18, 'Female', 3, '2006-04-19', 0),
(5, 16, 'Lebron', 'James', 24, 'Male', 5, '2000-12-30', 0),
(6, 16, 'Stephen', 'Curry', 20, 'Male', 5, '2004-03-14', 0),
(7, 19, 'Luka', 'Doncic', 17, 'Male', 6, '2007-02-28', 0),
(8, 19, 'Ariana', 'Grande', 23, 'Female', 7, '2001-06-26', 0),
(9, 58, 'Taylor', 'Swift', 25, 'Female', 8, '1999-12-13', 0),
(10, 58, 'Bruno', 'Mars', 26, 'Male', 9, '1998-10-08', 0),
(11, 5, 'Sarah', 'Geronimo', 28, 'Female', 1, '1996-07-25', 0),
(12, 13, 'Kathryn', 'Bernardo', 22, 'Female', 4, '2002-03-26', 0),
(13, 19, 'Daniel', 'Padilla', 23, 'Male', 6, '2001-04-26', 0),
(14, 58, 'Nadine', 'Lustre', 24, 'Female', 8, '2000-10-31', 0);

-- --------------------------------------------------------

--
-- Table structure for table `__efmigrationshistory`
--

CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `__efmigrationshistory`
--

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`) VALUES
('20251208104103_AddProjectDocuments', '9.0.10'),
('20251210042720_AddEmailVerification', '9.0.10');

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
  ADD KEY `fk_term_id` (`Term_ID`),
  ADD KEY `fk_compliance_template` (`TemplateFile_ID`);

--
-- Indexes for table `compliance_document`
--
ALTER TABLE `compliance_document`
  ADD PRIMARY KEY (`Document_ID`),
  ADD KEY `fk_compliance_doc_compliance` (`Compliance_ID`),
  ADD KEY `fk_compliance_doc_file` (`File_ID`);

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
-- Indexes for table `project_document`
--
ALTER TABLE `project_document`
  ADD PRIMARY KEY (`Document_ID`),
  ADD KEY `IX_project_document_File_ID` (`File_ID`),
  ADD KEY `IX_project_document_Project_ID` (`Project_ID`);

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
-- Indexes for table `__efmigrationshistory`
--
ALTER TABLE `__efmigrationshistory`
  ADD PRIMARY KEY (`MigrationId`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `announcement`
--
ALTER TABLE `announcement`
  MODIFY `Announcement_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `barangay`
--
ALTER TABLE `barangay`
  MODIFY `Barangay_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=64;

--
-- AUTO_INCREMENT for table `budget`
--
ALTER TABLE `budget`
  MODIFY `budget_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `compliance`
--
ALTER TABLE `compliance`
  MODIFY `com_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

--
-- AUTO_INCREMENT for table `compliance_document`
--
ALTER TABLE `compliance_document`
  MODIFY `Document_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=15;

--
-- AUTO_INCREMENT for table `federation_fund`
--
ALTER TABLE `federation_fund`
  MODIFY `Fund_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `file_upload`
--
ALTER TABLE `file_upload`
  MODIFY `File_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=45;

--
-- AUTO_INCREMENT for table `kabataan_service_record`
--
ALTER TABLE `kabataan_service_record`
  MODIFY `Record_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- AUTO_INCREMENT for table `kabataan_term_period`
--
ALTER TABLE `kabataan_term_period`
  MODIFY `Term_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `login`
--
ALTER TABLE `login`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=18;

--
-- AUTO_INCREMENT for table `project`
--
ALTER TABLE `project`
  MODIFY `Project_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;

--
-- AUTO_INCREMENT for table `project_allocation`
--
ALTER TABLE `project_allocation`
  MODIFY `Allocation_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=15;

--
-- AUTO_INCREMENT for table `project_document`
--
ALTER TABLE `project_document`
  MODIFY `Document_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=15;

--
-- AUTO_INCREMENT for table `project_log`
--
ALTER TABLE `project_log`
  MODIFY `Log_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=29;

--
-- AUTO_INCREMENT for table `role`
--
ALTER TABLE `role`
  MODIFY `Role_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `sitio`
--
ALTER TABLE `sitio`
  MODIFY `sitio_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT for table `system_log`
--
ALTER TABLE `system_log`
  MODIFY `SysLog_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=188;

--
-- AUTO_INCREMENT for table `user`
--
ALTER TABLE `user`
  MODIFY `User_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=18;

--
-- AUTO_INCREMENT for table `youth_member`
--
ALTER TABLE `youth_member`
  MODIFY `Member_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=15;

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
  ADD CONSTRAINT `fk_compliance_template` FOREIGN KEY (`TemplateFile_ID`) REFERENCES `file_upload` (`File_ID`) ON DELETE SET NULL,
  ADD CONSTRAINT `fk_file_id` FOREIGN KEY (`File_ID`) REFERENCES `file_upload` (`File_ID`) ON DELETE SET NULL ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_term_id` FOREIGN KEY (`Term_ID`) REFERENCES `kabataan_term_period` (`Term_ID`) ON UPDATE CASCADE;

--
-- Constraints for table `compliance_document`
--
ALTER TABLE `compliance_document`
  ADD CONSTRAINT `fk_compliance_doc_compliance` FOREIGN KEY (`Compliance_ID`) REFERENCES `compliance` (`com_id`) ON DELETE CASCADE,
  ADD CONSTRAINT `fk_compliance_doc_file` FOREIGN KEY (`File_ID`) REFERENCES `file_upload` (`File_ID`);

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
-- Constraints for table `project_document`
--
ALTER TABLE `project_document`
  ADD CONSTRAINT `FK_project_document_file_upload_File_ID` FOREIGN KEY (`File_ID`) REFERENCES `file_upload` (`File_ID`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_project_document_project_Project_ID` FOREIGN KEY (`Project_ID`) REFERENCES `project` (`Project_ID`) ON DELETE CASCADE;

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

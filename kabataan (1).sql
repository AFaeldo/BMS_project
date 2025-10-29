-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Oct 29, 2025 at 01:20 AM
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

-- --------------------------------------------------------

--
-- Table structure for table `budget_item`
--

CREATE TABLE `budget_item` (
  `Budget_ID` int(11) NOT NULL,
  `Project_ID` int(11) DEFAULT NULL,
  `Item_Name` varchar(255) NOT NULL,
  `Quantity` int(11) DEFAULT 1,
  `Unit_Cost` decimal(10,2) DEFAULT 0.00,
  `Total_Cost` decimal(10,2) GENERATED ALWAYS AS (`Quantity` * `Unit_Cost`) STORED
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `file_upload`
--

CREATE TABLE `file_upload` (
  `File_ID` int(11) NOT NULL,
  `File_Name` varchar(255) NOT NULL,
  `File` longblob NOT NULL,
  `File_Category` varchar(100) DEFAULT NULL,
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
  `password` varchar(50) NOT NULL,
  `Role_ID` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `login`
--

INSERT INTO `login` (`id`, `username`, `password`, `Role_ID`) VALUES
(4, 'brent', 'Dwcc@2025', 1),
(5, 'aiken', 'aiken', 2),
(6, 'kurt', 'kurt', 3);

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
  `Contact_No` varchar(20) DEFAULT NULL,
  `Barangay_ID` int(11) DEFAULT NULL,
  `Role_ID` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

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
-- Dumping data for table `youth_member`
--

INSERT INTO `youth_member` (`Member_ID`, `First_Name`, `Last_Name`, `Age`, `Gender`, `sitio`, `Birthday`, `Barangay_ID`) VALUES
(12, 'kurt', 'kurt', 0, 'Male', 'Kanluran', '2025-10-04', NULL),
(13, 'Nicole', 'Santiago', 20, 'Female', 'Lalud', '2005-10-03', NULL),
(14, 'kurt', 'angelo', 0, 'Female', 'asdasdas', '2025-10-05', NULL),
(15, 'Brent', 'Bolanos', 21, 'Male', 'Kanluran', '2004-06-07', NULL),
(16, 'arth', 'Ahorro', 0, 'Male', 'dsaasdsad', '2025-10-10', NULL);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `barangay`
--
ALTER TABLE `barangay`
  ADD PRIMARY KEY (`Barangay_ID`);

--
-- Indexes for table `budget_item`
--
ALTER TABLE `budget_item`
  ADD PRIMARY KEY (`Budget_ID`),
  ADD KEY `Project_ID` (`Project_ID`);

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
  ADD KEY `Role` (`Role_ID`);

--
-- Indexes for table `project`
--
ALTER TABLE `project`
  ADD PRIMARY KEY (`Project_ID`),
  ADD KEY `User_ID` (`User_ID`);

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
  MODIFY `Barangay_ID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `budget_item`
--
ALTER TABLE `budget_item`
  MODIFY `Budget_ID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `file_upload`
--
ALTER TABLE `file_upload`
  MODIFY `File_ID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `login`
--
ALTER TABLE `login`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT for table `project`
--
ALTER TABLE `project`
  MODIFY `Project_ID` int(11) NOT NULL AUTO_INCREMENT;

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
  MODIFY `User_ID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `youth_member`
--
ALTER TABLE `youth_member`
  MODIFY `Member_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `budget_item`
--
ALTER TABLE `budget_item`
  ADD CONSTRAINT `budget_item_ibfk_1` FOREIGN KEY (`Project_ID`) REFERENCES `project` (`Project_ID`) ON DELETE CASCADE ON UPDATE CASCADE;

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
  ADD CONSTRAINT `Role` FOREIGN KEY (`Role_ID`) REFERENCES `role` (`Role_ID`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `project`
--
ALTER TABLE `project`
  ADD CONSTRAINT `project_ibfk_1` FOREIGN KEY (`User_ID`) REFERENCES `user` (`User_ID`) ON DELETE CASCADE ON UPDATE CASCADE;

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

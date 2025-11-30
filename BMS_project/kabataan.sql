-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Nov 30, 2025 at 06:17 AM
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
  `Birthday` date NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `youth_member`
--

INSERT INTO `youth_member` (`Member_ID`, `Barangay_ID`, `First_Name`, `Last_Name`, `Age`, `Gender`, `sitio`, `Birthday`) VALUES
(2, 16, 'kurt', 'kurt', 17, 'Male', 'asdadssa', '2008-01-02');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `youth_member`
--
ALTER TABLE `youth_member`
  ADD PRIMARY KEY (`Member_ID`),
  ADD KEY `youth_member_ibfk_1` (`Barangay_ID`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `youth_member`
--
ALTER TABLE `youth_member`
  MODIFY `Member_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `youth_member`
--
ALTER TABLE `youth_member`
  ADD CONSTRAINT `barangay_id` FOREIGN KEY (`Barangay_ID`) REFERENCES `barangay` (`Barangay_ID`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

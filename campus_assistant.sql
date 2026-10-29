-- MySQL dump 10.13  Distrib 8.4.9, for Win64 (x86_64)
--
-- Host: localhost    Database: campus_assistant
-- ------------------------------------------------------
-- Server version	8.4.9

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Current Database: `campus_assistant`
--

CREATE DATABASE /*!32312 IF NOT EXISTS*/ `campus_assistant` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;

USE `campus_assistant`;

--
-- Table structure for table `activity_signup`
--

DROP TABLE IF EXISTS `activity_signup`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `activity_signup` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `activity_id` bigint NOT NULL,
  `user_id` bigint NOT NULL,
  `status` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `create_time` datetime(6) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `activity_signup`
--

LOCK TABLES `activity_signup` WRITE;
/*!40000 ALTER TABLE `activity_signup` DISABLE KEYS */;
INSERT INTO `activity_signup` VALUES (1,1,6,'signed','2026-10-11 18:12:18.281035'),(2,1,7,'signed','2026-10-11 18:12:18.281086'),(3,2,5,'signed','2026-10-11 18:12:18.281087'),(4,2,8,'signed','2026-10-11 18:12:18.281087'),(5,4,5,'signed','2026-10-11 18:12:18.281088'),(6,4,7,'cancelled','2026-10-11 18:12:18.281088'),(7,5,5,'signed','2026-10-28 22:57:43.523062');
/*!40000 ALTER TABLE `activity_signup` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ai_chat_log`
--

DROP TABLE IF EXISTS `ai_chat_log`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ai_chat_log` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `user_id` bigint NOT NULL,
  `question` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `answer` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `create_time` datetime(6) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ai_chat_log`
--

LOCK TABLES `ai_chat_log` WRITE;
/*!40000 ALTER TABLE `ai_chat_log` DISABLE KEYS */;
INSERT INTO `ai_chat_log` VALUES (1,5,'校园卡丢了怎么办','根据校园知识库：校园卡丢失后应立即在食堂圈存机或财务处网站挂失，补办需携带学生证到校园卡服务中心（行政楼105），工本费20元，3个工作日后取卡。','2026-10-09 21:12:17.046464'),(2,5,'怎么申请请假','学生请假需登录教务系统提交请假申请，写明请假事由与起止时间，由辅导员审批。3天以上需学院副书记审批，7天以上需教务处备案。','2026-10-09 22:12:17.046464'),(3,6,'图书馆能借几本书','本科生一次最多借阅10册，借期30天，可续借一次15天。逾期未还每册每天罚款0.1元。','2026-10-10 20:12:17.046464'),(4,7,'体测受伤了可以免测吗','因伤病无法参加体测的学生，需持二甲以上医院证明，填写免测申请表，经体育部审核通过后可免测，成绩按及格计。','2026-10-11 00:12:17.046464'),(5,5,'奖学金怎么申请','国家奖学金要求学年平均分85分以上且无挂科；励志奖学金面向家庭经济困难学生，平均分80分以上。每年9月开放申请。','2026-10-11 08:12:17.046464'),(6,10,'你好','你好呀！有什么校园生活方面的问题需要我帮忙解答吗？','2026-10-11 18:48:31.245342'),(7,10,'奖学金怎么申请','申请奖学金的步骤如下：\n\n1. **申请时间**：每年9月份左右，具体时间会通过学校通知。\n2. **申请条件**：\n   - **国家奖学金**：学年平均分要在85分以上，且没有挂科。\n   - **励志奖学金**：针对家庭经济困难的学生，平均分80分以上。\n3. **提交材料**：在学生工作系统提交申请材料。\n4. **注意事项**：具体要求和流程可能会有所变化，请关注学校发布的最新通知。\n\n记得提前准备好成绩单、家庭经济困难证明等相关材料哦！','2026-10-11 18:48:39.382157'),(8,5,'学校图书馆的开放时间是几点到几点？','图书馆的开放时间是每天早上7:30到晚上22:00。','2026-10-11 21:47:24.215095'),(9,5,'你好','你好呀！有什么校园生活方面的问题需要我帮忙解答吗？','2026-10-15 15:53:20.101467'),(10,5,'图书馆等校园信息','好的，关于图书馆的信息，我这里有几个重点：\n\n1. **借阅规则**：本科生一次最多可以借阅10本书，借期是30天，还可以续借一次，续借期限是15天。\n2. **罚款规定**：如果逾期未还，每本书每天要罚款0.1元。\n3. **开放时间**：图书馆的开放时间是周一到周日，每天7:30到22:00。\n\n还有其他关于图书馆或校园信息的问题吗？','2026-10-15 15:53:41.801318'),(11,5,'请假','请假的话，首先得登录教务系统，然后提交请假申请。在申请里要写清楚请假的原因和起止时间。如果请假时间超过3天，得让学院的副书记审批；如果超过7天，还得教务处备案。记得这些步骤哦！','2026-10-15 15:59:58.260872'),(12,5,'1','好的，学生朋友，如果你需要申请请假，这里有几个步骤：\n\n1. 登录教务系统。\n2. 在系统中填写请假申请，说明你的请假原因和起止时间。\n3. 提交申请后，由辅导员进行审批。\n4. 如果请假时间超过3天，需要学院副书记审批；超过7天，还需教务处备案。\n\n需要报修宿舍设施吗？告诉我具体问题，我帮你解答。','2026-10-28 14:08:12.511375'),(13,5,'心理咨询预约','心理咨询预约可以这样操作：\n1. 电话预约：拨打校内分机8888。\n2. 公众号预约：关注心理中心公众号，按提示操作。\n3. 咨询地点：大学生活动中心三楼。\n4. 注意：全程保密哦。','2026-10-28 22:55:03.407626');
/*!40000 ALTER TABLE `ai_chat_log` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ai_knowledge`
--

DROP TABLE IF EXISTS `ai_knowledge`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ai_knowledge` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `title` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `content` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `category` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `is_enabled` tinyint(1) NOT NULL,
  `create_time` datetime(6) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ai_knowledge`
--

LOCK TABLES `ai_knowledge` WRITE;
/*!40000 ALTER TABLE `ai_knowledge` DISABLE KEYS */;
INSERT INTO `ai_knowledge` VALUES (1,'如何申请请假','学生请假需登录教务系统提交请假申请，写明请假事由与起止时间，由辅导员审批。3天以上需学院副书记审批，7天以上需教务处备案。','请假',1,'2026-10-11 18:12:18.298901'),(2,'宿舍报修流程','宿舍设施损坏可在本平台移动端提交报修单，注明报修类型、宿舍地址和故障描述，后勤人员接单后上门维修，一般24小时内响应，紧急情况（漏水、漏电）2小时内响应。','报修',1,'2026-10-11 18:12:18.298951'),(3,'奖学金申请条件','国家奖学金要求学年平均分85分以上且无挂科；励志奖学金面向家庭经济困难学生，平均分80分以上。每年9月开放申请，需在学生工作系统提交材料。','奖助',1,'2026-10-11 18:12:18.298951'),(4,'图书馆借阅规则','本科生一次最多借阅10册，借期30天，可续借一次15天。逾期未还每册每天罚款0.1元。图书馆开放时间：周一至周日 7:30-22:00。','图书馆',1,'2026-10-11 18:12:18.298951'),(5,'校园卡挂失补办','校园卡丢失后应立即在食堂圈存机或财务处网站挂失，补办需携带学生证到校园卡服务中心（行政楼105），工本费20元，3个工作日后取卡。','校园卡',1,'2026-10-11 18:12:18.298951'),(6,'体测免测申请','因伤病无法参加体测的学生，需持二甲以上医院证明，填写免测申请表，经体育部审核通过后可免测，成绩按及格计。每学期体测前两周截止申请。','体育',1,'2026-10-11 18:12:18.298951'),(7,'转专业政策','大一学年结束后可申请转专业，要求大一学年平均分75分以上且无挂科。每年6月发布接收计划，需参加接收学院的考核（笔试或面试）。','教务',1,'2026-10-11 18:12:18.298952'),(8,'勤工助学岗位申请','家庭经济困难学生可申请图书馆助理、实验室助理等勤工助学岗位，时薪15元，每月不超过40小时。在学生资助管理中心网站报名。','奖助',1,'2026-10-11 18:12:18.298952'),(9,'四六级报名与成绩查询','四六级每年6月、12月各考一次，报名在教务系统开放，报名费30元。成绩一般在考后两个月公布，可在教务系统或中国教育考试网查询。','考试',1,'2026-10-11 18:12:18.298952'),(10,'宿舍用电安全规定','宿舍限电800W，禁止使用热得快、电炉、电饭煲等大功率电器，违者没收电器并通报批评。宿舍无人时请关闭电源，充电器不要长时间插在插座上。','宿舍',1,'2026-10-11 18:12:18.298952'),(11,'心理咨询预约','校心理健康教育中心提供免费心理咨询，可通过电话（校内分机8888）或心理中心公众号预约，咨询地点：大学生活动中心三楼，全程保密。','心理',1,'2026-10-11 18:12:18.298952'),(12,'火车票学生优惠卡','学生证火车票优惠卡每学年需充值一次（4次单程优惠），充值地点：教务处学籍科。优惠区间仅限学校至家庭所在地，乘车时需携带学生证。','生活',1,'2026-10-11 18:12:18.298952');
/*!40000 ALTER TABLE `ai_knowledge` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `alert_record`
--

DROP TABLE IF EXISTS `alert_record`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `alert_record` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `student_id` bigint NOT NULL,
  `course_name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `score_value` decimal(5,1) NOT NULL,
  `reason` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `is_read` tinyint(1) NOT NULL,
  `create_time` datetime(6) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `alert_record`
--

LOCK TABLES `alert_record` WRITE;
/*!40000 ALTER TABLE `alert_record` DISABLE KEYS */;
INSERT INTO `alert_record` VALUES (1,5,'大学英语(三)',55.0,'成绩低于60分',0,'2026-10-08 18:12:17.046464'),(2,5,'数据结构',58.0,'成绩低于60分',0,'2026-10-10 18:12:17.046464'),(3,6,'高等数学(上)',42.0,'成绩低于60分',1,'2026-10-06 18:12:17.046464'),(4,7,'大学物理(下)',47.0,'成绩低于60分',0,'2026-10-09 18:12:17.046464');
/*!40000 ALTER TABLE `alert_record` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `announcement`
--

DROP TABLE IF EXISTS `announcement`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `announcement` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `title` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `content` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `category` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `scope` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `status` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `is_top` tinyint(1) NOT NULL,
  `publisher_id` bigint NOT NULL,
  `publisher_name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `view_count` int NOT NULL,
  `create_time` datetime(6) NOT NULL,
  `update_time` datetime(6) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `announcement`
--

LOCK TABLES `announcement` WRITE;
/*!40000 ALTER TABLE `announcement` DISABLE KEYS */;
INSERT INTO `announcement` VALUES (1,'关于2025-2026学年第一学期期末考试安排的通知','各位同学：本学期期末考试将于第18-19周进行，具体时间地点请登录教务系统查询。请提前做好复习准备，考试时须携带学生证，严禁携带手机等电子设备进入考场。','teaching','all','published',1,1,'系统管理员',329,'2026-10-06 18:12:17.046464','2026-10-11 18:12:18.338926'),(2,'校园网络设备升级维护公告','为提升校园网络质量，信息中心将于本周六 00:00-06:00 对西区宿舍楼网络设备进行升级，期间可能短暂断网，请提前保存工作。','notice','all','published',0,1,'系统管理员',158,'2026-10-08 18:12:17.046464','2026-10-11 18:12:18.339048'),(3,'关于举办第十六届校园文化艺术节的通知','本届校园文化艺术节将于下月开幕，包含歌手大赛、书画展、摄影比赛、话剧专场等活动，欢迎各班级积极报名参与。','activity','all','published',0,2,'王建国',245,'2026-10-09 18:12:17.046464','2026-10-11 18:12:18.339048'),(4,'宿舍冬季用电安全提醒','天气转冷，请同学们规范用电，严禁使用热得快、电炉等大功率电器，离开宿舍请关闭电源，注意防火安全。','life','all','published',0,1,'系统管理员',89,'2026-10-10 18:12:17.046464','2026-10-11 18:12:18.339049'),(5,'计算机学院关于组织毕业设计中期检查的通知','请2022级同学于下周五前提交毕业设计中期报告，指导教师审核后统一提交学院。','teaching','dept','pending',0,3,'李慧敏',0,'2026-10-11 12:12:17.046464','2026-10-11 18:12:18.339049'),(7,'w','11111','notice','all','rejected',1,2,'王建',0,'2026-10-15 16:01:26.361717','2026-10-15 16:02:59.630039');
/*!40000 ALTER TABLE `announcement` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cafeteria_menu`
--

DROP TABLE IF EXISTS `cafeteria_menu`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cafeteria_menu` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `canteen` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `window` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `meal_type` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `dish_name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `price` decimal(6,1) NOT NULL,
  `description` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `create_time` datetime(6) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cafeteria_menu`
--

LOCK TABLES `cafeteria_menu` WRITE;
/*!40000 ALTER TABLE `cafeteria_menu` DISABLE KEYS */;
INSERT INTO `cafeteria_menu` VALUES (1,'一食堂','家常热菜','午餐','红烧肉盖饭',14.0,'肥瘦相间，配青菜','2026-10-11 18:12:18.349560'),(2,'一食堂','家常热菜','午餐','番茄炒蛋盖饭',10.0,'酸甜可口','2026-10-11 18:12:18.349663'),(3,'一食堂','面食窗口','午餐','牛肉拉面',12.0,'现拉现煮','2026-10-11 18:12:18.349664'),(4,'一食堂','早点档','早餐','小笼包(6个)',5.0,'鲜肉小笼包','2026-10-11 18:12:18.349664'),(5,'一食堂','早点档','早餐','豆浆+油条',4.0,'经典搭配','2026-10-11 18:12:18.349669'),(6,'二食堂','麻辣香锅','午餐','麻辣香锅(自选)',16.0,'荤素搭配，可加辣','2026-10-11 18:12:18.349669'),(7,'二食堂','轻食沙拉','晚餐','鸡胸肉沙拉',15.0,'低脂高蛋白','2026-10-11 18:12:18.349669'),(8,'二食堂','特色小吃','晚餐','酸辣粉',8.0,'重庆风味','2026-10-11 18:12:18.349669'),(9,'二食堂','特色小吃','晚餐','烤冷面',7.0,'东北特色','2026-10-11 18:12:18.349669'),(10,'民族餐厅','清真窗口','午餐','大盘鸡拌面',18.0,'分量足','2026-10-11 18:12:18.349669');
/*!40000 ALTER TABLE `cafeteria_menu` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `campus_activity`
--

DROP TABLE IF EXISTS `campus_activity`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `campus_activity` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `title` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `content` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `location` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `start_time` datetime(6) DEFAULT NULL,
  `end_time` datetime(6) DEFAULT NULL,
  `quota` int NOT NULL,
  `status` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `creator_id` bigint NOT NULL,
  `create_time` datetime(6) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `campus_activity`
--

LOCK TABLES `campus_activity` WRITE;
/*!40000 ALTER TABLE `campus_activity` DISABLE KEYS */;
INSERT INTO `campus_activity` VALUES (1,'校园歌手大赛','一年一度的校园歌手大赛开始报名！初赛自选曲目清唱90秒，决赛乐队伴奏。设一等奖1名、二等奖2名、三等奖3名，获奖可加综合素质分。','学术报告厅','2026-10-21 18:12:17.046464',NULL,100,'published',1,'2026-10-07 18:12:17.046464'),(2,'编程马拉松 Hackathon','24小时极限编程挑战，三人一队，主题现场公布。提供夜宵和咖啡，冠军队伍奖金3000元+实习内推机会。','实验楼','2026-10-26 18:12:17.046464',NULL,60,'published',1,'2026-10-08 18:12:17.046464'),(3,'秋季篮球联赛','以学院为单位组队报名，每队8-12人，小组赛+淘汰赛制。请队长统一报名并提交队员名单。','东区篮球场','2026-10-18 18:12:17.046464',NULL,80,'published',2,'2026-10-09 18:12:17.046464'),(4,'心理健康讲座：如何应对考前焦虑','校心理中心专职教师主讲，现场有互动答疑环节，参加可盖讲座章。','图书馆报告厅','2026-10-14 18:12:17.046464',NULL,150,'published',3,'2026-10-10 18:12:17.046464'),(5,'1','1','1','2026-10-14 16:00:00.000000',NULL,1,'published',2,'2026-10-15 15:58:35.653565'),(6,'篮球社社团活动','篮球活动','篮球场1号场','2026-11-01 01:00:00.000000',NULL,30,'published',1,'2026-10-28 16:11:11.885436');
/*!40000 ALTER TABLE `campus_activity` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `consultation`
--

DROP TABLE IF EXISTS `consultation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `consultation` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `user_id` bigint NOT NULL,
  `question` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `category` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `reply` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `replier_id` bigint DEFAULT NULL,
  `replier_name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `status` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `create_time` datetime(6) NOT NULL,
  `reply_time` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `consultation`
--

LOCK TABLES `consultation` WRITE;
/*!40000 ALTER TABLE `consultation` DISABLE KEYS */;
INSERT INTO `consultation` VALUES (1,5,'我的毕业设计题目偏难，想换一个方向可以吗？','学习',NULL,NULL,NULL,'pending','2026-10-11 14:12:17.046464',NULL),(2,6,'最近总是失眠焦虑，学校有心理咨询吗？','心理',NULL,NULL,NULL,'pending','2026-10-10 18:12:17.046464',NULL),(3,7,'宿舍限电800W，我想买个吹风机可以吗？','生活','宿舍可以使用功率不超过800W的吹风机，但请勿同时使用多个大功率电器，注意用电安全。',2,'王建国','replied','2026-10-09 18:12:17.046464','2026-10-09 21:12:17.046464'),(4,5,'如何申请国家奖学金？','学习','hhxx',2,'王建国','replied','2026-10-11 18:12:51.712085','2026-10-11 18:50:36.433542'),(5,5,'图书馆等校园信息','学习','好的',2,'王建','replied','2026-10-15 15:54:05.075621','2026-10-15 15:54:43.918862'),(6,5,'1','学习','111',2,'王建','replied','2026-10-15 16:00:58.447284','2026-10-15 16:01:35.607759');
/*!40000 ALTER TABLE `consultation` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `course`
--

DROP TABLE IF EXISTS `course`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `course` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `teacher_id` bigint NOT NULL,
  `classroom` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `schedule` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `semester` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `credit` int NOT NULL,
  `create_time` datetime(6) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `course`
--

LOCK TABLES `course` WRITE;
/*!40000 ALTER TABLE `course` DISABLE KEYS */;
INSERT INTO `course` VALUES (1,'高等数学(上)',2,'教一101','周一1-2节','2025-2026-1',4,'2026-10-11 18:12:18.155245'),(2,'大学英语(三)',3,'教二203','周三3-4节','2025-2026-1',3,'2026-10-11 18:12:18.155336'),(3,'Web 应用开发',2,'实验楼305','周五5-6节','2025-2026-1',3,'2026-10-11 18:12:18.155337'),(4,'数据结构',2,'教三402','周二7-8节','2025-2026-1',4,'2026-10-11 18:12:18.155337'),(5,'大学物理(下)',3,'教一305','周四3-4节','2025-2026-1',3,'2026-10-11 18:12:18.155337');
/*!40000 ALTER TABLE `course` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `login_log`
--

DROP TABLE IF EXISTS `login_log`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `login_log` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `user_id` bigint DEFAULT NULL,
  `username` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ip` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `result` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `message` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `create_time` datetime(6) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=143 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `login_log`
--

LOCK TABLES `login_log` WRITE;
/*!40000 ALTER TABLE `login_log` DISABLE KEYS */;
INSERT INTO `login_log` VALUES (1,1,'admin','127.0.0.1','success','登录成功','2026-10-11 10:12:17.046464'),(2,5,'student','192.168.1.23','success','登录成功','2026-10-11 12:12:17.046464'),(3,NULL,'student','192.168.1.50','fail','密码错误','2026-10-11 15:12:17.046464'),(4,4,'logistics','192.168.1.88','success','登录成功','2026-10-11 16:12:17.046464'),(5,2,'teacher','192.168.1.11','success','登录成功','2026-10-11 17:12:17.046464'),(6,5,'student','::1','success','登录成功','2026-10-11 18:12:51.097412'),(7,2,'teacher','::1','success','登录成功','2026-10-11 18:12:51.222580'),(8,1,'admin','::1','success','登录成功','2026-10-11 18:12:51.359917'),(9,4,'logistics','::1','success','登录成功','2026-10-11 18:12:51.504954'),(10,5,'student','::1','success','登录成功','2026-10-11 18:12:59.304935'),(11,1,'admin','::1','success','登录成功','2026-10-11 18:12:59.446827'),(12,4,'logistics','::1','success','登录成功','2026-10-11 18:12:59.590339'),(13,5,'student','::1','success','登录成功','2026-10-11 18:20:47.799804'),(14,5,'student','::1','success','登录成功','2026-10-11 18:20:48.157540'),(15,1,'admin','::1','success','登录成功','2026-10-11 18:20:48.368201'),(16,2,'teacher','::1','success','登录成功','2026-10-11 18:20:48.569495'),(17,4,'logistics','::1','success','登录成功','2026-10-11 18:20:48.764640'),(18,5,'student','::1','success','登录成功','2026-10-11 18:20:48.955173'),(19,5,'student','::1','success','登录成功','2026-10-11 18:21:45.806043'),(20,5,'student','::1','success','登录成功','2026-10-11 18:21:52.895677'),(21,5,'student','::1','success','登录成功','2026-10-11 18:22:54.622660'),(22,10,'tan','::1','success','登录成功','2026-10-11 18:48:05.700502'),(23,2,'teacher','::1','success','登录成功','2026-10-11 18:49:47.515847'),(24,4,'logistics','::1','success','登录成功','2026-10-11 18:52:45.326183'),(25,1,'admin','::1','success','登录成功','2026-10-11 18:53:22.940718'),(26,5,'student','::1','success','登录成功','2026-10-11 21:01:34.055514'),(27,5,'student','::1','success','登录成功','2026-10-11 21:01:43.452130'),(28,5,'student','::1','success','登录成功','2026-10-11 21:41:19.611601'),(29,1,'admin','::1','success','登录成功','2026-10-11 21:43:49.827838'),(30,5,'student','::1','success','登录成功','2026-10-11 21:44:00.950730'),(31,1,'admin','::1','success','登录成功','2026-10-11 21:46:49.949743'),(32,5,'student','::1','success','登录成功','2026-10-11 21:46:58.127442'),(33,1,'admin','::1','success','登录成功','2026-10-11 21:47:29.104772'),(34,5,'student','::1','success','登录成功','2026-10-11 21:47:40.185563'),(35,5,'student','::1','success','登录成功','2026-10-11 21:51:20.920897'),(36,5,'student','::1','success','登录成功','2026-10-11 21:59:41.346909'),(37,5,'student','::1','success','登录成功','2026-10-11 22:02:43.492301'),(38,1,'admin','::1','success','登录成功','2026-10-15 14:18:27.921703'),(39,1,'admin','::1','success','登录成功','2026-10-15 14:28:09.266030'),(40,10,'tan','::1','success','登录成功','2026-10-15 15:18:43.622470'),(41,NULL,' student ','::1','fail','用户名或密码错误','2026-10-15 15:20:53.274254'),(42,NULL,' student ','::1','fail','用户名或密码错误','2026-10-15 15:21:12.767760'),(43,NULL,' student ','::1','fail','用户名或密码错误','2026-10-15 15:21:18.541563'),(44,NULL,'teacher ','::1','fail','用户名或密码错误','2026-10-15 15:21:51.091405'),(45,NULL,'teacher ','::1','fail','用户名或密码错误','2026-10-15 15:22:06.564109'),(46,1,'admin','::1','success','登录成功','2026-10-15 15:22:56.166084'),(47,2,'teacher','::1','success','登录成功','2026-10-15 15:22:56.296541'),(48,3,'teacher2','::1','success','登录成功','2026-10-15 15:22:56.435110'),(49,4,'logistics','::1','success','登录成功','2026-10-15 15:22:56.574104'),(50,5,'student','::1','success','登录成功','2026-10-15 15:22:56.697049'),(51,6,'student2','::1','success','登录成功','2026-10-15 15:22:56.844798'),(52,7,'student3','::1','success','登录成功','2026-10-15 15:22:56.971302'),(53,8,'student4','::1','success','登录成功','2026-10-15 15:22:57.118812'),(54,NULL,'newstu01','::1','fail','用户名或密码错误','2026-10-15 15:22:57.245742'),(55,NULL,'tan','::1','fail','用户名或密码错误','2026-10-15 15:22:57.370390'),(56,NULL,'谭浩杰','::1','fail','用户名或密码错误','2026-10-15 15:28:16.581620'),(57,1,'admin','::1','success','登录成功','2026-10-15 15:30:40.636422'),(58,5,'student','::1','success','登录成功','2026-10-15 15:30:40.961389'),(59,5,'student','::1','success','登录成功','2026-10-15 15:33:37.380754'),(60,11,'123','::1','success','登录成功','2026-10-15 15:39:14.218345'),(61,NULL,'123','::1','fail','用户名或密码错误','2026-10-15 15:40:46.989065'),(62,11,'123','::1','success','登录成功','2026-10-15 15:40:51.207000'),(63,NULL,'log','::1','fail','用户名或密码错误','2026-10-15 15:41:21.114197'),(64,4,'logistics','::1','success','登录成功','2026-10-15 15:41:43.262410'),(65,2,'teacher','::1','success','登录成功','2026-10-15 15:43:03.466208'),(66,2,'teacher','::1','success','登录成功','2026-10-15 15:44:11.036759'),(67,5,'student','::1','success','登录成功','2026-10-15 15:44:32.002157'),(68,4,'logistics','::1','success','登录成功','2026-10-15 15:45:34.523672'),(69,1,'admin','::1','success','登录成功','2026-10-15 15:47:07.546589'),(70,12,'1234','::1','success','登录成功','2026-10-15 15:52:11.622082'),(71,5,'student','::1','success','登录成功','2026-10-15 15:52:30.750832'),(72,2,'teacher','::1','success','登录成功','2026-10-15 15:54:21.255317'),(73,NULL,'student','::1','fail','用户名或密码错误','2026-10-15 15:55:04.820349'),(74,5,'student','::1','success','登录成功','2026-10-15 15:55:15.585063'),(75,5,'student','::1','success','登录成功','2026-10-15 15:56:36.474062'),(76,2,'teacher','::1','success','登录成功','2026-10-15 15:57:02.511430'),(77,1,'admin','::1','success','登录成功','2026-10-15 15:57:35.337101'),(78,2,'teacher','::1','success','登录成功','2026-10-15 15:58:05.453551'),(79,5,'student','::1','success','登录成功','2026-10-15 15:59:20.714465'),(80,5,'student','::1','success','登录成功','2026-10-15 16:00:45.971776'),(81,2,'teacher','::1','success','登录成功','2026-10-15 16:01:13.117562'),(82,1,'admin','::1','success','登录成功','2026-10-15 16:02:12.142295'),(83,4,'logistics','::1','success','登录成功','2026-10-15 16:03:56.481518'),(84,NULL,'tan','::1','fail','用户名或密码错误','2026-10-21 21:36:41.349608'),(85,10,'tan','::1','success','登录成功','2026-10-21 21:36:47.093561'),(86,1,'admin','::1','success','登录成功','2026-10-21 21:37:05.374999'),(87,NULL,'student','::1','fail','用户名或密码错误','2026-10-21 21:38:06.516828'),(88,5,'student','::1','success','登录成功','2026-10-21 21:43:06.781886'),(89,5,'student','::1','success','登录成功','2026-10-24 20:16:20.983992'),(90,1,'admin','::1','success','登录成功','2026-10-24 21:16:05.758674'),(91,5,'student','::1','success','登录成功','2026-10-24 21:22:56.958369'),(92,5,'student','::1','success','登录成功','2026-10-27 20:50:14.080454'),(93,NULL,'log','::1','fail','用户名或密码错误','2026-10-27 20:50:40.866147'),(94,2,'teacher','::1','success','登录成功','2026-10-27 20:52:16.764179'),(95,4,'logistics','::1','success','登录成功','2026-10-27 21:00:47.099272'),(96,4,'张维','::1','success','登录成功','2026-10-27 21:01:31.266582'),(97,5,'谭浩杰','::1','success','登录成功','2026-10-27 21:02:31.326596'),(98,2,'王建','::1','success','登录成功','2026-10-27 21:02:59.047171'),(99,NULL,'teacher','::1','fail','用户名或密码错误','2026-10-28 14:04:33.289825'),(100,NULL,'王维','::1','fail','用户名或密码错误','2026-10-28 14:04:45.519416'),(101,NULL,'王维','::1','fail','用户名或密码错误','2026-10-28 14:04:49.966820'),(102,5,'谭浩杰','::1','success','登录成功','2026-10-28 14:06:59.236021'),(103,2,'王建','::1','success','登录成功','2026-10-28 14:09:27.983478'),(104,5,'谭浩杰','::1','success','登录成功','2026-10-28 15:59:53.320235'),(105,5,'谭浩杰','::1','success','登录成功','2026-10-28 16:00:37.735177'),(106,1,'admin','::1','success','登录成功','2026-10-28 16:07:57.620134'),(107,2,'王建','::1','success','登录成功','2026-10-28 16:14:47.416099'),(108,NULL,'student','::1','fail','用户名或密码错误','2026-10-28 16:20:34.887590'),(109,1,'admin','::1','success','登录成功','2026-10-28 16:21:03.327932'),(110,NULL,'teacher','::1','fail','用户名或密码错误','2026-10-28 16:21:03.409181'),(111,NULL,'logistics','::1','fail','用户名或密码错误','2026-10-28 16:21:03.434642'),(112,6,'student2','::1','success','登录成功','2026-10-28 16:21:03.605105'),(113,NULL,'student','::1','fail','用户名或密码错误','2026-10-28 16:21:03.642176'),(114,6,'student2','::1','success','登录成功','2026-10-28 16:21:16.515114'),(115,6,'student2','::1','success','登录成功','2026-10-28 16:21:28.582919'),(116,1,'admin','::1','success','登录成功','2026-10-28 16:21:43.067308'),(117,1,'admin','::1','success','登录成功','2026-10-28 16:27:37.643016'),(118,NULL,'teacher','::1','fail','用户名或密码错误','2026-10-28 16:27:38.177012'),(119,1,'admin','::1','success','登录成功','2026-10-28 16:27:49.811785'),(120,NULL,'??','::1','fail','用户名或密码错误','2026-10-28 16:28:30.884405'),(121,NULL,'??','::1','fail','用户名或密码错误','2026-10-28 16:28:31.012860'),(122,NULL,'???','::1','fail','用户名或密码错误','2026-10-28 16:28:31.031135'),(123,3,'teacher2','::1','success','登录成功','2026-10-28 16:28:31.206210'),(124,6,'student2','::1','success','登录成功','2026-10-28 16:28:48.316665'),(125,3,'teacher2','::1','success','登录成功','2026-10-28 16:28:48.597677'),(126,5,'谭浩杰','::1','success','登录成功','2026-10-28 17:24:15.454747'),(127,1,'admin','::1','success','登录成功','2026-10-28 17:26:15.624509'),(128,5,'谭浩杰','::1','success','登录成功','2026-10-28 21:19:45.838367'),(129,1,'admin','::1','success','登录成功','2026-10-28 22:02:21.907431'),(130,12,'1234','::1','fail','账号已被禁用','2026-10-28 22:02:56.631350'),(131,1,'admin','::1','success','登录成功','2026-10-28 22:03:11.530950'),(132,NULL,'王伟','::1','fail','用户名或密码错误','2026-10-28 22:42:35.236978'),(133,4,'张维','::1','success','登录成功','2026-10-28 22:42:48.602302'),(134,4,'张维','::1','success','登录成功','2026-10-28 22:45:48.873353'),(135,2,'王建','::1','success','登录成功','2026-10-28 22:51:13.986795'),(136,1,'admin','::1','success','登录成功','2026-10-28 22:51:49.203419'),(137,1,'admin','::1','success','登录成功','2026-10-28 23:03:44.849131'),(138,5,'谭浩杰','::1','success','登录成功','2026-10-28 23:21:19.923104'),(139,5,'谭浩杰','::1','success','登录成功','2026-10-28 23:22:51.651459'),(140,2,'王建','::1','success','登录成功','2026-10-28 23:34:00.494590'),(141,1,'admin','::1','success','登录成功','2026-10-28 23:34:16.697244'),(142,NULL,'student','::1','fail','用户名或密码错误','2026-10-29 00:40:09.210931');
/*!40000 ALTER TABLE `login_log` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `lost_found`
--

DROP TABLE IF EXISTS `lost_found`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `lost_found` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `user_id` bigint NOT NULL,
  `type` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `title` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `location` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `image` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `contact` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `status` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `create_time` datetime(6) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `lost_found`
--

LOCK TABLES `lost_found` WRITE;
/*!40000 ALTER TABLE `lost_found` DISABLE KEYS */;
INSERT INTO `lost_found` VALUES (1,6,'found','捡到一副黑色蓝牙耳机','在教一101最后一排桌兜里捡到，黑色入耳式，带充电盒，品牌看不清','教一101',NULL,'13900000002','active','2026-10-09 18:12:17.046464'),(2,7,'found','捡到校园卡一张','卡面姓名：陈xx，在食堂二楼捡到，已交至食堂服务台','食堂二楼',NULL,'食堂服务台','active','2026-10-10 18:12:17.046464'),(3,5,'lost','丢失蓝色保温杯','深蓝色膳魔师保温杯，杯身贴有姓名贴，可能落在图书馆','图书馆三楼',NULL,'13900000001','active','2026-10-10 18:12:17.046464'),(4,8,'lost','丢失学生证','陈晓雯，计算机学院，可能在操场或体育馆丢失','东区操场',NULL,'13900000004','active','2026-10-11 10:12:17.046464'),(5,6,'found','捡到一把格子雨伞','灰蓝色格子折叠伞，在教学楼主入口伞架上','教一正门',NULL,'13900000002','closed','2026-10-05 18:12:17.046464');
/*!40000 ALTER TABLE `lost_found` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `notification`
--

DROP TABLE IF EXISTS `notification`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `notification` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `user_id` bigint NOT NULL,
  `title` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `content` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `type` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `is_read` tinyint(1) NOT NULL,
  `create_time` datetime(6) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `notification`
--

LOCK TABLES `notification` WRITE;
/*!40000 ALTER TABLE `notification` DISABLE KEYS */;
INSERT INTO `notification` VALUES (1,5,'报修工单已提交','您的报修「宿舍洗手池水管漏水」已提交，后勤正在安排处理。','repair',0,'2026-10-11 16:12:17.046464'),(2,5,'学业预警提醒','您在《数据结构》的成绩为58分，已触发学业预警，建议尽快联系任课老师。','ai',1,'2026-10-10 18:12:17.046464'),(3,5,'期末考试安排已发布','本学期期末考试安排已发布，请登录教务系统查询。','announce',0,'2026-10-06 18:12:17.046464'),(4,4,'新报修工单','东区3栋502 提交了新的报修工单（水电类，紧急度：高），请及时处理。','repair',0,'2026-10-11 16:12:17.046464'),(5,4,'工单待接单提醒','当前有 2 条待接单工单，请及时处理。','repair',1,'2026-10-11 12:12:17.046464'),(6,2,'学生咨询待回复','有新的学生咨询问题待回复。','system',1,'2026-10-11 14:12:17.046464'),(7,1,'公告待审核','李慧敏 提交了公告《计算机学院关于组织毕业设计中期检查的通知》，等待审核。','announce',0,'2026-10-11 12:12:17.046464'),(8,2,'学生咨询待回复','有新的学生咨询（学习）：如何申请国家奖学金？','system',1,'2026-10-11 18:12:51.725864'),(9,3,'学生咨询待回复','有新的学生咨询（学习）：如何申请国家奖学金？','system',0,'2026-10-11 18:12:51.725978'),(10,4,'新报修工单','东区3栋501 提交了新的报修工单（家具类，紧急度：中），请及时处理。','repair',0,'2026-10-11 18:13:02.001989'),(11,5,'报修进度更新','您的报修工单 #6（东区3栋501）已接单，维修中。','repair',0,'2026-10-11 18:13:02.044591'),(12,5,'报修进度更新','您的报修工单 #6（东区3栋501）已完工。','repair',0,'2026-10-11 18:13:02.137700'),(13,5,'咨询已回复','您的咨询已由 王建国 回复：hhxx','system',1,'2026-10-11 18:50:36.434924'),(14,4,'新报修工单','学生公寓3栋303 提交了新的报修工单（水电维修类，紧急度：高），请及时处理。','repair',0,'2026-10-11 21:47:13.246200'),(15,4,'新报修工单','学生公寓3栋303 提交了新的报修工单（水电维修类，紧急度：高），请及时处理。','repair',0,'2026-10-11 21:51:36.589244'),(16,4,'新报修工单','学生公寓3栋303 提交了新的报修工单（水电维修类，紧急度：高），请及时处理。','repair',1,'2026-10-11 22:03:00.215235'),(17,4,'工单分配提醒','工单 #9（学生公寓3栋303）已分配给您，请及时处理。','repair',0,'2026-10-15 15:45:47.838549'),(18,5,'报修进度更新','您的报修工单 #9（学生公寓3栋303）已接单，维修中。','repair',1,'2026-10-15 15:45:47.848505'),(19,2,'学生咨询待回复','有新的学生咨询（学习）：图书馆等校园信息','system',0,'2026-10-15 15:54:05.103973'),(20,3,'学生咨询待回复','有新的学生咨询（学习）：图书馆等校园信息','system',0,'2026-10-15 15:54:05.104319'),(21,5,'咨询已回复','您的咨询已由 王建 回复：好的','system',1,'2026-10-15 15:54:43.920283'),(22,1,'公告待审核','王建 提交了公告《1》，等待审核。','announce',0,'2026-10-15 15:57:16.808654'),(23,2,'学生咨询待回复','有新的学生咨询（学习）：1','system',0,'2026-10-15 16:00:58.455965'),(24,3,'学生咨询待回复','有新的学生咨询（学习）：1','system',0,'2026-10-15 16:00:58.456063'),(25,1,'公告待审核','王建 提交了公告《w》，等待审核。','announce',0,'2026-10-15 16:01:26.376645'),(26,5,'咨询已回复','您的咨询已由 王建 回复：111','system',0,'2026-10-15 16:01:35.608813'),(27,5,'报修进度更新','您的报修工单 #9（学生公寓3栋303）已完工。','repair',0,'2026-10-15 16:04:01.118411'),(28,4,'新报修工单','a2107 提交了新的报修工单（水电类，紧急度：中），请及时处理。','repair',0,'2026-10-28 22:41:56.723455'),(29,5,'报修进度更新','您的报修工单 #10（a2107）已接单，维修中。','repair',0,'2026-10-28 22:42:59.977689'),(30,5,'报修进度更新','您的报修工单 #10（a2107）已完工。','repair',0,'2026-10-28 22:43:01.540536'),(31,4,'新报修工单','a2107 提交了新的报修工单（网络类，紧急度：中），请及时处理。','repair',0,'2026-10-28 22:45:12.999363');
/*!40000 ALTER TABLE `notification` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `operation_log`
--

DROP TABLE IF EXISTS `operation_log`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `operation_log` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `user_id` bigint DEFAULT NULL,
  `username` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `module` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `action` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `detail` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ip` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `create_time` datetime(6) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `operation_log`
--

LOCK TABLES `operation_log` WRITE;
/*!40000 ALTER TABLE `operation_log` DISABLE KEYS */;
INSERT INTO `operation_log` VALUES (1,1,'admin','用户管理','新增用户','创建教师账号 teacher2','127.0.0.1','2026-10-09 18:12:17.046464'),(2,4,'logistics','报修工单','接单','接单工单 #2（网络类）','192.168.1.88','2026-10-10 22:12:17.046464'),(3,4,'logistics','报修工单','完工','完成工单 #3（门窗类）','192.168.1.88','2026-10-09 18:12:17.046464'),(4,1,'admin','公告管理','发布公告','发布全校公告《期末考试安排》','127.0.0.1','2026-10-06 18:12:17.046464'),(5,2,'teacher','公告管理','发布公告','《1》（pending）','::1','2026-10-15 15:57:16.769577'),(6,2,'teacher','公告管理','发布公告','《w》（pending）','::1','2026-10-15 16:01:26.364085'),(7,1,'admin','用户管理','禁用账号','1234','::1','2026-10-28 22:02:38.416873'),(8,1,'admin','用户管理','启用账号','1234','::1','2026-10-28 22:03:16.106779'),(9,1,'admin','用户管理','禁用账号','1234','::1','2026-10-28 23:34:20.931894');
/*!40000 ALTER TABLE `operation_log` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `repair_order`
--

DROP TABLE IF EXISTS `repair_order`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `repair_order` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `user_id` bigint NOT NULL,
  `category` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `dormitory` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `images` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `status` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `priority` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ai_advice` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `handler_id` bigint DEFAULT NULL,
  `contact` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `rating` int DEFAULT NULL,
  `comment` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `review_time` datetime(6) DEFAULT NULL,
  `create_time` datetime(6) NOT NULL,
  `update_time` datetime(6) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `repair_order`
--

LOCK TABLES `repair_order` WRITE;
/*!40000 ALTER TABLE `repair_order` DISABLE KEYS */;
INSERT INTO `repair_order` VALUES (1,5,'水电','东区3栋502','宿舍洗手池水管漏水，地面积水严重',NULL,'pending','高','建议先关闭入户水阀，师傅携带管钳与生料带上门。',NULL,NULL,NULL,NULL,NULL,'2026-10-11 16:12:17.046464','2026-10-11 18:12:18.197656'),(2,6,'网络','西区7栋318','宿舍WiFi信号特别差，经常断连',NULL,'processing','中','建议检查楼道交换机端口，必要时重启AP。',4,NULL,NULL,NULL,NULL,'2026-10-10 18:12:17.046464','2026-10-11 18:12:18.197775'),(3,7,'门窗','东区5栋210','阳台推拉门轨道卡死，开关很费力',NULL,'done','低','清理轨道杂物并加润滑油即可。',4,NULL,NULL,NULL,NULL,'2026-10-08 18:12:17.046464','2026-10-09 18:12:17.046464'),(4,8,'水电','西区2栋106','宿舍顶灯闪烁，疑似镇流器老化',NULL,'done','中','更换同型号镇流器或整体更换LED灯板。',4,NULL,NULL,NULL,NULL,'2026-10-07 18:12:17.046464','2026-10-08 18:12:17.046464'),(5,6,'家具','西区7栋318','上床下桌的书桌抽屉滑轨脱落',NULL,'pending','低','重新固定滑轨螺丝，必要时更换滑轨。',NULL,NULL,NULL,NULL,NULL,'2026-10-11 12:12:17.046464','2026-10-11 18:12:18.197808'),(6,5,'家具','东区3栋501','空调不制冷，吹出来都是热风',NULL,'done','中','请检查空调电源是否正常，如问题依旧，请关闭电源等待维修师傅上门。',4,'13900000001',5,'师傅很专业，态度好','2026-10-11 18:13:02.205076','2026-10-11 18:13:01.991607','2026-10-11 18:13:02.137626'),(7,5,'水电维修','学生公寓3栋303','卫生间水龙头持续漏水，影响正常使用，请尽快安排维修',NULL,'pending','高','请检查水龙头密封圈是否损坏，如需更换请及时更换。',NULL,'13800000001',NULL,NULL,NULL,'2026-10-11 21:47:13.186457','2026-10-11 21:47:13.186457'),(8,5,'水电维修','学生公寓3栋303','卫生间水龙头持续漏水，影响正常使用，请尽快安排维修',NULL,'pending','高','请师傅检查水龙头密封件，尽快修复漏水问题。',NULL,'13800000001',NULL,NULL,NULL,'2026-10-11 21:51:36.574441','2026-10-11 21:51:36.574441'),(9,5,'水电维修','学生公寓3栋303','卫生间水龙头持续漏水，影响正常使用，请尽快安排维修',NULL,'done','高','请师傅检查水龙头密封圈是否损坏，尽快修复以防止水资源浪费。',4,'13800000001',NULL,NULL,NULL,'2026-10-11 22:03:00.205556','2026-10-15 16:04:01.118329'),(10,5,'水电','a2107','水龙头漏水','','done','中','请师傅尽快修理，以免浪费水资源。',4,'17823146495',NULL,NULL,NULL,'2026-10-28 22:41:56.620408','2026-10-28 22:43:01.539316'),(11,5,'网络','a2107','网络不好','','pending','中','请检查网络连接线是否松动，如果问题依旧，请等待维修师傅上门。',NULL,'1',NULL,NULL,NULL,'2026-10-28 22:45:12.979540','2026-10-28 22:45:12.979541');
/*!40000 ALTER TABLE `repair_order` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `repair_progress`
--

DROP TABLE IF EXISTS `repair_progress`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `repair_progress` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `repair_id` bigint NOT NULL,
  `status` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `remark` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `images` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `operator_id` bigint NOT NULL,
  `operator_name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `create_time` datetime(6) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `repair_progress`
--

LOCK TABLES `repair_progress` WRITE;
/*!40000 ALTER TABLE `repair_progress` DISABLE KEYS */;
INSERT INTO `repair_progress` VALUES (1,1,'pending','学生提交报修，等待后勤接单',NULL,5,'谭浩杰','2026-10-11 16:12:17.046464'),(2,2,'pending','学生提交报修，等待后勤接单',NULL,6,'张三','2026-10-10 18:12:17.046464'),(3,2,'processing','已接单，携带网线测试仪上门排查',NULL,4,'张维修','2026-10-10 22:12:17.046464'),(4,3,'processing','已接单，准备轨道润滑油',NULL,4,'张维修','2026-10-08 22:12:17.046464'),(5,3,'done','轨道清理并上油，推拉顺畅，维修完成',NULL,4,'张维修','2026-10-09 18:12:17.046464'),(6,4,'done','更换LED灯板，通电测试正常',NULL,4,'张维修','2026-10-08 18:12:17.046464'),(7,6,'pending','学生提交报修，等待后勤接单',NULL,5,'student','2026-10-11 18:13:01.999397'),(8,6,'processing','已接单，带氟利昂上门',NULL,4,'logistics','2026-10-11 18:13:02.044478'),(9,6,'processing','检测到缺氟，正在加注',NULL,4,'logistics','2026-10-11 18:13:02.095967'),(10,6,'done','加氟完成，制冷正常',NULL,4,'logistics','2026-10-11 18:13:02.137636'),(11,7,'pending','学生提交报修，等待后勤接单',NULL,5,'student','2026-10-11 21:47:13.230543'),(12,8,'pending','学生提交报修，等待后勤接单',NULL,5,'student','2026-10-11 21:51:36.588131'),(13,9,'pending','学生提交报修，等待后勤接单',NULL,5,'student','2026-10-11 22:03:00.214292'),(14,9,'processing','1',NULL,4,'张维','2026-10-15 15:45:47.840832'),(15,9,'processing','ww','',4,'logistics','2026-10-15 15:46:02.309275'),(16,9,'done',NULL,NULL,4,'logistics','2026-10-15 16:04:01.118332'),(17,10,'pending','学生提交报修，等待后勤接单',NULL,5,'谭浩杰','2026-10-28 22:41:56.679957'),(18,10,'processing',NULL,NULL,4,'张维','2026-10-28 22:42:59.977457'),(19,10,'done',NULL,NULL,4,'张维','2026-10-28 22:43:01.539322'),(20,11,'pending','学生提交报修，等待后勤接单',NULL,5,'谭浩杰','2026-10-28 22:45:12.994579');
/*!40000 ALTER TABLE `repair_progress` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `score`
--

DROP TABLE IF EXISTS `score`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `score` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `student_id` bigint NOT NULL,
  `course_id` bigint NOT NULL,
  `value` decimal(5,1) NOT NULL,
  `semester` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `create_time` datetime(6) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `score`
--

LOCK TABLES `score` WRITE;
/*!40000 ALTER TABLE `score` DISABLE KEYS */;
INSERT INTO `score` VALUES (1,5,1,88.0,'2025-2026-1','2026-10-11 18:12:18.182738'),(2,5,2,55.0,'2025-2026-1','2026-10-11 18:12:18.182811'),(3,5,3,92.0,'2025-2026-1','2026-10-11 18:12:18.182811'),(4,5,4,58.0,'2025-2026-1','2026-10-11 18:12:18.182811'),(5,6,1,42.0,'2025-2026-1','2026-10-11 18:12:18.182817'),(6,6,4,71.0,'2025-2026-1','2026-10-11 18:12:18.182817'),(7,7,1,79.0,'2025-2026-1','2026-10-11 18:12:18.182818'),(8,7,2,83.0,'2025-2026-1','2026-10-11 18:12:18.182818'),(9,7,5,47.0,'2025-2026-1','2026-10-11 18:12:18.182818'),(10,8,2,90.0,'2025-2026-1','2026-10-11 18:12:18.182818'),(11,8,3,85.0,'2025-2026-1','2026-10-11 18:12:18.182818');
/*!40000 ALTER TABLE `score` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `student_course`
--

DROP TABLE IF EXISTS `student_course`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `student_course` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `student_id` bigint NOT NULL,
  `course_id` bigint NOT NULL,
  `create_time` datetime(6) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `student_course`
--

LOCK TABLES `student_course` WRITE;
/*!40000 ALTER TABLE `student_course` DISABLE KEYS */;
INSERT INTO `student_course` VALUES (1,5,1,'2026-10-11 18:12:18.177364'),(2,5,2,'2026-10-11 18:12:18.177413'),(3,5,3,'2026-10-11 18:12:18.177413'),(4,5,4,'2026-10-11 18:12:18.177413'),(5,6,1,'2026-10-11 18:12:18.177413'),(6,6,4,'2026-10-11 18:12:18.177413'),(7,7,1,'2026-10-11 18:12:18.177414'),(8,7,2,'2026-10-11 18:12:18.177414'),(9,7,5,'2026-10-11 18:12:18.177415'),(10,8,2,'2026-10-11 18:12:18.177416'),(11,8,3,'2026-10-11 18:12:18.177416');
/*!40000 ALTER TABLE `student_course` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sys_user`
--

DROP TABLE IF EXISTS `sys_user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sys_user` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `username` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `password` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `role` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `student_no` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `phone` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `email` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `status` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `create_time` datetime(6) NOT NULL,
  `update_time` datetime(6) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sys_user`
--

LOCK TABLES `sys_user` WRITE;
/*!40000 ALTER TABLE `sys_user` DISABLE KEYS */;
INSERT INTO `sys_user` VALUES (1,'admin','$2a$11$wqKtEO7IylGq9HF2it3OG.4l7VhfkbZyfPpo/ScKMWJ4wj1zJsQB2','系统管理员','admin',NULL,NULL,NULL,'enabled','2026-10-11 18:12:17.046530','2026-10-11 18:12:17.046530'),(2,'王建','$2a$11$Agpi7/6IT7sDSeoiRTW.se4SZYzBZCsJBsHMvrRmqnc6OCcmfIBfG','王建','teacher',NULL,'13800000001',NULL,'enabled','2026-10-11 18:12:17.219156','2026-10-15 15:43:58.856727'),(3,'teacher2','$2a$11$YWODOx1haLZQ7KKut3eD8e43eLbkSgMISaMYkf3ddj90X2IaI3y1.','李慧','teacher',NULL,'13800000002',NULL,'enabled','2026-10-11 18:12:17.365843','2026-10-11 18:12:17.365847'),(4,'张维','$2a$11$8dXAZHe9uQPsU1AIN4CQ7e3egt4x2KlaKeelh96U.B6cV8Ph2DgkW','张维','logistics',NULL,'13800000003',NULL,'enabled','2026-10-11 18:12:17.483146','2026-10-11 18:53:07.809703'),(5,'谭浩杰','$2a$11$/aJp0.BPvxAtkTT5XXyAb.dqPTSrmkDu12yk1G0qllAvAVuZOjy16','谭浩杰','student','20250751119','17823111111',NULL,'enabled','2026-10-11 18:12:17.594238','2026-10-15 16:00:34.926066'),(6,'student2','$2a$11$jTKIZ1KWgonBxjL8EclCHOMeN9cHan4mC92PVEDMQlYrQ3ZP2K2ze','张三','student','20250751120','13900000002',NULL,'enabled','2026-10-11 18:12:17.701273','2026-10-11 18:12:17.701277'),(7,'student3','$2a$11$INhnH1qqLIUHoErfrSAJGurP5FSMMAEJXsoHu4/LfqIZGhe8QFlH.','刘思远','student','20250751121','13900000003',NULL,'enabled','2026-10-11 18:12:17.808695','2026-10-11 18:12:17.808698'),(8,'student4','$2a$11$ZD1g28osGGQWFLinFBPpw.SB5W7t/95WAmix3pgF4IhLXvzacGT0C','陈晓雯','student','20250751122','13900000004',NULL,'enabled','2026-10-11 18:12:17.917445','2026-10-11 18:12:17.917447'),(9,'newstu01','$2a$11$Ne4ZXKGQUTpEs7Q/fJ3E/OGWGUFhyKnvClpmSa7bSAk0FqXW5IlGK','测试新生','student','20250759999','13700001111',NULL,'enabled','2026-10-11 18:12:43.445451','2026-10-11 18:12:43.445451'),(10,'tan','$2a$11$leU.3IuesqxyCMtdKwP.IeOUyM8zNIQ0pDwr8rWdF2r5M.NXb.NU.','谭浩杰','student','20250751119','17823146495',NULL,'enabled','2026-10-11 18:47:55.613550','2026-10-11 18:47:55.613556'),(11,'123','$2a$11$t4w4N7rQmqtDH2Xx12yXde5GPNhRc8yxyJRe3jsdexdR6AP6Wt5kC','123456','student','20250751118','17823146496',NULL,'enabled','2026-10-15 15:39:08.386317','2026-10-15 15:40:39.324241'),(12,'1234','$2a$11$sRqXLFkNXEyiuB7591kATOJiCHVJkx9TIzdCMeWIv.ggvKn59Btf6','12345','student','20250751117','17823146493',NULL,'disabled','2026-10-15 15:52:07.375515','2026-10-28 23:34:20.931327');
/*!40000 ALTER TABLE `sys_user` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `venue_booking`
--

DROP TABLE IF EXISTS `venue_booking`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `venue_booking` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `venue_id` bigint NOT NULL,
  `user_id` bigint NOT NULL,
  `start_time` datetime(6) NOT NULL,
  `end_time` datetime(6) NOT NULL,
  `reason` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `status` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `create_time` datetime(6) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `venue_booking`
--

LOCK TABLES `venue_booking` WRITE;
/*!40000 ALTER TABLE `venue_booking` DISABLE KEYS */;
INSERT INTO `venue_booking` VALUES (1,1,6,'2026-10-13 14:00:00.000000','2026-10-13 16:00:00.000000','社团换届大会','pending','2026-10-11 13:12:17.046464'),(2,3,5,'2026-10-12 19:00:00.000000','2026-10-12 21:00:00.000000','毕设小组讨论','approved','2026-10-10 18:12:17.046464'),(3,2,7,'2026-10-10 18:00:00.000000','2026-10-10 20:00:00.000000','班级篮球训练','approved','2026-10-09 18:12:17.046464'),(4,4,8,'2026-10-14 09:00:00.000000','2026-10-14 12:00:00.000000','舞蹈社排练','rejected','2026-10-10 18:12:17.046464'),(7,1,5,'2026-11-28 19:00:00.000000','2026-11-28 21:00:00.000000','观众','pending','2026-10-28 17:25:10.803489');
/*!40000 ALTER TABLE `venue_booking` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `venue_resource`
--

DROP TABLE IF EXISTS `venue_resource`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `venue_resource` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `location` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `capacity` int NOT NULL,
  `equipment` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `create_time` datetime(6) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `venue_resource`
--

LOCK TABLES `venue_resource` WRITE;
/*!40000 ALTER TABLE `venue_resource` DISABLE KEYS */;
INSERT INTO `venue_resource` VALUES (1,'学术报告厅','图书馆一楼',200,'投影、音响、舞台灯光','2026-10-11 18:12:18.286086'),(2,'篮球场1号场','东区体育场',30,'夜间灯光','2026-10-11 18:12:18.286144'),(3,'研讨室A','图书馆三楼',12,'白板、电视投屏','2026-10-11 18:12:18.286145'),(4,'多功能厅','大学生活动中心二楼',80,'投影、音响、桌椅可移动','2026-10-11 18:12:18.286145'),(5,'琴房3号','艺术楼一层',4,'立式钢琴','2026-10-11 18:12:18.286145');
/*!40000 ALTER TABLE `venue_resource` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-10-29  0:43:20

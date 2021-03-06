DROP TABLE IF EXISTS `sys_user`;
create table `sys_user`(
	`id` int not null auto_increment comment '唯一标识',
	`name` varchar(16) not null comment '名称',
	`nickName` varchar(16) null comment '昵称',
	`passWordMD5` varchar(64) null comment '密码（md5加密两次）',
	`mobile` varchar(11) null comment '手机号码',
	`email` varchar(64) null comment '邮箱',
	`headImgUrl` varchar(255) null comment '用户头像地址',
	`unionId` varchar(125) null comment '微信开放平台唯一Id',
	`sex` varchar(4) not null default '未知'  comment '性别 1男 2女 0未知',
	`area` varchar(125) null comment '所在地区； 中国-湖北-武汉',
	`permissions` varchar(5000) null comment '拥有的权限',
	`roleIds` varchar(1000) null comment '所属的角色',
	`countryId` int comment '国家id',
	`provinceId` int comment '省份',
	`cityId` int comment '城市',
	`districtId` int comment '地区',
	`createdBy` int null ,
	`createdTime` datetime null,
	`modifiedBy` int null ,
	`modifiedTime` datetime null,
	`deletedBy` int null ,
	`deletedTime` datetime DEFAULT NULL,
	`isDeleted` tinyint not null default 0,
	primary key (`id`)
) comment='用户表';




-- ----------------------------
-- Table structure for sys_menu
-- ----------------------------
DROP TABLE IF EXISTS `sys_menu`;
CREATE TABLE `sys_menu` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `PId` int(11) default 0 NULL comment '上级菜单Id 一级菜单默人为0',
  `Name` varchar(255) NOT NULL comment '菜单名称',
  `Component` varchar(255) NULL comment '菜单路径 一级菜单不填',
  `Alias` varchar(128) NULL comment '别名',
  `Redirect` varchar(255) NULL comment '重定向',
  `Path` varchar(255) NOT NULL comment '虚拟路径',
  `Title` varchar(128) NULL comment '标题',
	`Sort` int null comment '排序',
  `Icon` varchar(128) NOT NULL comment 'Icon',
  `AlwaysShow` tinyint NULL default 0 comment '是否一直显示',
  `Permissions` varchar(5000) DEFAULT NULL comment '读取菜单所需权限',
  `Description` varchar(500) DEFAULT NULL comment '菜单描述',
  `Hide` tinyint not null default 0 comment '是否隐藏 默认不隐藏 0',
  `CreatedBy` int(32) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedBy` int(32) DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  `DeletedBy` int(32) DEFAULT NULL,
  `DeletedTime` datetime DEFAULT NULL,
	`isDeleted` tinyint not null default 0,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='菜单';



-- ----------------------------
-- Table structure for sys_role
-- ----------------------------
DROP TABLE IF EXISTS `sys_role`;
CREATE TABLE `sys_role` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `PId` int(11) DEFAULT NULL,
  `Name` varchar(125) NOT NULL,
  `Permissions` varchar(5000) DEFAULT NULL,
  `Description` varchar(500) DEFAULT NULL,
  `CreatedBy` int(32) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedBy` int(32) DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  `DeletedBy` int(32) DEFAULT NULL,
  `DeletedTime` datetime DEFAULT NULL,
	`isDeleted` tinyint not null default 0,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='角色';



drop table if exists `sys_userInfo`;
create table `sys_userInfo`(
	`Id` int not null auto_increment,
	`UserId` varchar(32) not null comment '用户唯一标识',
	`OpenId` varchar(32) null comment '唯一标识',
	`ProjectId` varchar(32) null comment '项目唯一标识',
	`ProjectType` varchar(32) not null comment '项目类型',
	`CreatedBy` int null ,
	`CreatedTime` datetime null,
	`ModifiedBy` int null ,
	`ModifiedTime` datetime null,
	`DeletedBy` int null ,
	`DeletedTime` datetime DEFAULT NULL,
	`IsDeleted` tinyint not null default 0,
	primary key (`id`)
) comment='用户详情信息';


drop table if exists `sys_userTagInfo`;
create table `sys_userTagInfo`(
	`id` int not null auto_increment comment 'id',
	`name` varchar(125) not null comment '标签名称',
	`pId` int null comment '上级id',
	`createdBy` int null ,
	`createdTime` datetime null,
	`modifiedBy` int null ,
	`modifiedTime` datetime null,
	`deletedBy` int null ,
	`deletedTime` datetime DEFAULT NULL,
	`isDeleted` tinyint not null default 0,
	primary key (`id`)
) comment='用户标签';


drop table if exists `sys_log_api`;
create table `sys_log_api`(
	`id` int not null auto_increment comment 'id',
	`userName` varchar(125) null comment '用户名称',
	`userId` int null comment '',
	`startTime` datetime null comment '响应开始时间',
	`EndTime` datetime null comment '响应结束时间',
	`From` varchar(225) not null comment '来自哪儿',
	`Url` varchar(225) not null comment '请求路径',
	`HttpMethod` varchar(225) not null comment '请求类型',
	`LogLevel` varchar(24) not null comment '日志等级',
	`ElapsedTime` double not null comment '响应时长（秒）',
	`RequestParameter` varchar(1000)  null comment '请求参数',
	`ResponseParameter` varchar(1000)  null comment '返回参数',
	`ExceptionInfo` varchar(1000)  null comment '错误信息',
	`UserIP` varchar(225)  null comment '用户IP',
	`ServiceIP` varchar(225) not null comment '服务器IP',
	`BrowserType` varchar(500) not null comment '浏览器信息',
	primary key (`id`)
) comment='api访问表';


-- ----------------------------
-- Table structure for sys_log_event
-- ----------------------------
DROP TABLE IF EXISTS `sys_log_event`;
CREATE TABLE `sys_log_event` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `WriteDate` datetime NOT NULL COMMENT '记录时间',
  `UserId` int(11) NOT NULL COMMENT '用户id',
  `UserName` varchar(255) NOT NULL COMMENT '用户名称',
  `EventType` varchar(50) NOT NULL COMMENT '事件类型',
  `Content` varchar(500) NOT NULL COMMENT '事件详情',
  `Remark` varchar(1000) NOT NULL COMMENT '事件备注',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=53 DEFAULT CHARSET=utf8 COMMENT='事件日志';

-- ----------------------------
-- Table structure for sys_log_error
-- ----------------------------
DROP TABLE IF EXISTS `sys_log_error`;
CREATE TABLE `sys_log_error` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserName` varchar(125) DEFAULT NULL COMMENT '用户名称',
  `MsgType` varchar(255) DEFAULT NULL COMMENT '消息类型',
  `Message` varchar(255) DEFAULT NULL COMMENT '消息内容',
  `LogLevel` varchar(125) DEFAULT NULL COMMENT '消息内容',
  `Path` varchar(255) DEFAULT NULL COMMENT '请求路径',
  `Assembly` varchar(255) DEFAULT NULL COMMENT '程序集名称',
  `ActionArguments` varchar(255) DEFAULT NULL COMMENT '异常参数',
  `HttpMethod` varchar(255) DEFAULT NULL COMMENT '请求类型',
  `StackTrace` varchar(4000) DEFAULT NULL COMMENT '异常堆栈',
  `Source` varchar(255) DEFAULT NULL COMMENT '异常源',
  `IP` varchar(255) DEFAULT NULL COMMENT '服务器IP 端口',
  `UserAgent` varchar(255) DEFAULT NULL COMMENT '客户端浏览器标识',
  `ShowException` bit(1) DEFAULT NULL COMMENT '是否显示异常界面',
  `Time` varchar(255) DEFAULT NULL COMMENT '异常发生时间',
  `Method` varchar(255) DEFAULT NULL COMMENT '异常发生方法',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=25 DEFAULT CHARSET=utf8 COMMENT='系统错误日志';




drop table if exists `sys_attachment`;
create table `sys_attachment`(
	`Id` int not null auto_increment comment 'id',
	`Name` varchar(125) not null comment '附件名称',
	`Path` varchar(255) not null comment '相对地址',
	`Size` int null comment '文件大小',
	`Type` int null comment '文件类型',
	`ContentType` varchar(125) null comment '文件类型',
	`Extension` varchar(64) comment '后缀名',
	`Base64` varchar(500) comment 'base64 url',
	`enabled` tinyint not null default 0 comment '是否启用，0：启用；1禁用；',
	`CreatedBy` int null ,
	`CreatedTime` datetime null,
	`ModifiedBy` int null ,
	`ModifiedTime` datetime null,
	`DeletedBy` int null ,
	`DeletedTime` datetime DEFAULT NULL,
	`IsDeleted` tinyint not null default 0,
	primary key (`id`)
) comment='附件';


drop table if exists `sys_configuration`;
create table `sys_configuration`(
	`Id` int not null auto_increment comment 'id',
	`key` varchar(125) not null comment 'key',
	`value` varchar(1000) not null comment 'value',
	`Description` varchar(255) null comment 'Description',
	`CreatedBy` int null ,
	`CreatedTime` datetime null,
	`ModifiedBy` int null ,
	`ModifiedTime` datetime null,
	`DeletedBy` int null ,
	`DeletedTime` datetime DEFAULT NULL,
	`IsDeleted` tinyint not null default 0,
	primary key (`id`)
) comment='附件';


--CREATE DATABASE QL_THU_BONG
--USE QL_THU_BONG
--drop database QL_THU_BONG

--DROP TABLE IF EXISTS OrderDetail;
--DROP TABLE IF EXISTS ShoppingCartItem;

--DROP TABLE IF EXISTS Rating;
--DROP TABLE IF EXISTS Discount;
--DROP TABLE IF EXISTS ProductImages;
--DROP TABLE IF EXISTS Product_Category;
--DROP TABLE IF EXISTS Orders;
--DROP TABLE IF EXISTS ShoppingCart;
--DROP TABLE IF EXISTS ProductSize;

--DROP TABLE IF EXISTS Users;

--DROP TABLE IF EXISTS Product;
--DROP TABLE IF EXISTS Category;
--DROP TABLE IF EXISTS Roles;
GO

go
CREATE TABLE Category
(
	CategoryID varchar(20),
	CategoryName nvarchar(50),
	CategoryImage varchar(255),
	IsActive Bit,
	CategoryParentID varchar(20),
	Primary key(CategoryID),
	foreign key(CategoryParentID) references Category(CategoryID)
)
go
CREATE TABLE Product
(
	ProductID int IDENTITY(1,1),
	ProductName nvarchar(100),
	Isenabled int,
	Primary key(ProductID)
)

go
CREATE TABLE Product_Category
(
	ProductID int,
	CategoryID varchar(20),
	primary key(ProductID, CategoryID),
	foreign key(ProductID) references Product(ProductID),
	foreign key(CategoryID) references Category(CategoryID)
)
go
CREATE TABLE ProductSize
(
	ProductSizeID int IDENTITY(1,1),
	ProductID int,
	SizeName int,
	Price decimal,
	StockQuantity int,
	Primary key(ProductSizeID),
	Foreign key(ProductID) references Product(ProductID)
)
go
CREATE TABLE ProductImages
(
	ProductImageID int IDENTITY(1,1),
	ProductID int,
	ImageURL varchar(500),
	primary key(ProductImageID),
	foreign key(ProductID) references Product(ProductID)
)
go
CREATE TABLE Discount
(
	DiscountID int IDENTITY(1,1),
	ProductID int,
	DiscountName nvarchar(50),
	StartDate date, 
	EndDate date,
	DiscountRate float,
	primary key(DiscountID),
	foreign key(ProductID) references Product(ProductID)
)
go
CREATE TABLE Roles
(
	RoleID int IDENTITY(1,1),
	RoleName nvarchar(50),
	primary key(RoleID),
)
go
CREATE TABLE Users
(
	UserID int IDENTITY(1,1),
	RoleID int,
	Name nvarchar(50),
	SDT varchar(20),
	Password varchar(255),
	Email varchar(50),
	primary key(UserID),
	foreign key(RoleID) references Roles(RoleID)
)
ALTER TABLE Users 
ADD ResetPasswordCode NVARCHAR(100) NULL,
    ResetPasswordExpiry DATETIME NULL;

go
CREATE TABLE Rating
(
	RatingID int IDENTITY(1,1),
	ProductID int,
	UserID int,
	Star int,
	Comment nvarchar(500),
	ProductSizeName int,
	RatingDate date,
	primary key(RatingID),
	foreign key(ProductID) references Product(ProductID),
	foreign key(UserID) references Users(UserID)
)

go
CREATE TABLE Orders
(
	OrderID int IDENTITY(1,1),
	UserID int,
	OrderDate datetime,
	Address nvarchar(255),
	Status nvarchar(50),
	UserPaymentMethod nvarchar(50),
	SDT varchar(20),
	NameUser nvarchar(60),
	Discount decimal,
	primary key(OrderID),
	foreign key(UserID) references Users(UserID)
)

go
CREATE TABLE OrderDetail
(
	OrderID int not null,
	ProductSizeID int not null,
	Quantity int,
	UnitPrice decimal,
	primary key(OrderID, ProductSizeID),
	foreign key(ProductSizeID) references ProductSize(ProductSizeID),
	foreign key(OrderID) references Orders(OrderID)
)
go
CREATE TABLE ShoppingCart
(
	ShoppingCartID int IDENTITY(1,1),
	UserID int,
	primary key(ShoppingCartID),
	foreign key(UserID) references Users(UserID)
)
go
CREATE TABLE ShoppingCartItem
(
	ShoppingCartID int,
	ProductSizeID int,
	Quantity int,
	UnitPrice money,
	primary key(ShoppingCartID, ProductSizeID),
	foreign key(ProductSizeID) references ProductSize(ProductSizeID),
	foreign key(ShoppingCartID) references ShoppingCart(ShoppingCartID)
)

go
--------------------------------------------
INSERT INTO Category
values
('thubong', N'Thú Bông',null, 1, null),--1 thu bong
('hoathinh', N'Gấu Bông Hoạt Hình',null, 1, null),--2 hoat hinh
('goiom',N'Gối ôm',null, 1, null),--3
('gaubong',N'Gấu Bông',null, 1, null),--4
('2in1',N'Gấu Bông Có Mền 2IN1',null, 1, 'goiom'),--5 Men 2in1
('sizenho',N'Gấu Bông Size Nhỏ',null, 1, null),--6 size nho
('giare',N'Gấu Bông Giá Rẻ',null, 1, null),--7 gia re
('teddy',N'Gấu Bông Teddy',null, 1, null),--8 teddy
('dai',N'Gấu Bông Dài',null, 1, null),--9 dai
('thunhoibong',N'Thú Nhồi Bông',null, 1, 'thubong'),--10 thu nhoi bong
('traicaybong', N'Trái Cây Bông',null, 1, 'thubong'),--11 trai cay bong
('thucung', N'Thú Cưng', null, 1, 'thubong'),
('goichuu',N'Gối Chữ U',null, 1, 'goiom'), -- 12 chu u
('sizelon',N'Gấu Bông Size Lớn',null, 1, null), --13 size lon
('sinhnhat',N'Gấu Bông Sinh Nhật','gau-bong-lotso-om-banh-kem-dau-1-768x768.jpg', 1, null), --14 sinh nhat
('totnghiep',N'Gấu Bông Tốt Nghiệp','gaubong-teddy-totnghiep-long-xoan-1.jpg', 1, null), --15 tot nghiep
('noel',N'Gấu Bông Noel','gaubong-ong-gia-noel-goi-om-dut-tay-1-400x400.jpg', 1, null), -- 16 noel
('heo',N'Heo Bông','heo-bong-tiktok-dung-mac-ao-soc.jpg', 1, 'thucung'), --17
('tho',N'Thỏ Bông','gaubong-baby-three-galaxy-5.jpg', 1, 'thunhoibong'), -- 18
('hoa',N'Hoa gấu bông','gau-bong-hoa-hong-ngan-dung-qua-1.jpg', 1, 'traicaybong'), --19
('meo',N'Mèo Bông','meo-bong-hoang-thuong-cosplay-ech-1.jpg', 1, 'thucung'), -- 20
('cho',N'Chó Bông','cho-bong-om-hoa-1.jpg', 1, 'thucung'), -- 21
('huoucaoco',N'Gấu Bông Hươu Cao Cổ','gau-bong-huou-cao-co-v2.jpg', 1, 'thunhoibong'), -- 22
('chimcanhcut',N'Chim Cánh Cụt','gau-bong-chim-canh-cut.jpg', 1, 'thunhoibong'), -- 23
('vit',N'Vịt Bông','vit-bong-tram-cam-trang-long-smooth-6.jpg', 1, 'thunhoibong'), -- 24
('raica',N'Rái Cá Bông','gau-bong-rai-ca-deo-headphone-nau-1-2048x2048.jpg', 1, 'thunhoibong'), -- 25
('voi',N'Voi Bông','khung-long-bong-ikea-hong.jpg', 1, 'thunhoibong'), -- 26
('panda',N'Gấu Panda','gau-truc-panda-om-binh-sua-2-768x768.jpg', 1, 'thunhoibong'), -- 27
('husky',N'Chó Bông Husky','goi-om-tron-cho-husky.jpg', 1, 'thunhoibong'), -- 28
('duahau',N'Dưa Hấu Bông','gau-bong-mieng-dua-hau-1.jpg', 1, 'traicaybong'), -- 29
('saurieng',N'Gấu Bông Trái Sầu Riêng','gau-bong-trai-sau-rieng-1.jpg', 1, 'traicaybong'), -- 30
('bo',N'Gấu Bông Trái Bơ','goi-men-trai-bo-bong-2in1-tay-theu.jpg', 1, 'traicaybong'), -- 31
('chuoi',N'Chuối Bông','trai-chuoi-bong-vang-3.jpg', 1, 'traicaybong'), -- 32
('capybara',N'Gấu Bông Capybara','capybara-thoi-keo-sin-gum-3.jpg', 1, 'thunhoibong'), --33
('shin',N'Gấu Bông Shin','shin-hoc-sinh-mini.jpg', 1, 'hoathinh'), --34
('doraemon',N'Gấu Bông Doraemon','gau-bong-doremon-long-ni-cam-tim.jpg', 1, 'hoathinh'), --35
('labubu',N'Gấu Bông Labubu','gau-bong-labubu-cosplay-ca-map-2.jpg', 1, 'hoathinh'), --36
('loopy',N'Gấu Bông Hải Ly Loopy','gaubong-loopy-hong-cospplay-capybara-doi-cam-2-300x300.jpg', 1, 'hoathinh'), --37
('lotso',N'Gấu Bông Lotso','goi-men-mat-gau-bong-lotso-1.jpg', 1, 'hoathinh'), --38
('lena',N'Gấu Bông Lena','gau-bong-lena-nam-cosplay-stitch-xanh-1.jpg', 1, 'hoathinh'), --39
('stitch',N'Gấu Bông Stitch','gau-bong-stitch-xanh-an-kem-1.jpg', 1, 'hoathinh'), --40
('kuromi',N'Gấu Bông Kuromi','gaubong-kuromi-doi-co-men-2in1-1.jpg', 1, 'hoathinh'), --41
('kitty',N'Gấu Bông Hello Kitty','kitty-om-bo.jpg', 1, 'hoathinh'), --42
('melody',N'Thỏ Bông Melody','tho-bong-melody-nam-ngu-2.jpg', 1, 'hoathinh'), --43
('cinnamoroll',N'Thỏ Bông Cinnamoroll','gau-bong-Cinnamoroll-co-men-2in1-galaxy.jpg', 1, 'hoathinh'), --44
('babythree', N'Gấu Bông Baby Three','gau-bong-baby-three-cosplay-shiba-5.jpg', 1, 'hoathinh'), --45
('haicau', N'Gấu Bông Hải Cẩu','gau-bong-hai-cau-sushi-trang-2.jpg', 1, 'thunhoibong')
go 
INSERT INTO Product
values
(N'Gấu Bông Stitch Hồng lông Smooth'),
(N'Gấu Bông Stitch Xanh cosplay Monster'),
(N'Gấu Bông Stitch nằm lông mịn Smooth'),
(N'Gấu Bông Stitch Xanh Mặc Yếm Jeans'),

(N'Gấu Bông Hoa Hồng Khổng Lồ có ngăn Giấu Quà'),
(N'Hoa Gấu Bông Lotso'),
(N'Hoa Gấu Bông Lena'),
(N'Gấu Bông Hoa Hồng Khổng Lồ'),

(N'Vịt Bông cosplay Súp Lơ xanh lông mịn'),
(N'Gấu Bông Hươu Cao Cổ lông Smooth siêu mịn'),
(N'Gấu Bông Capybara mũm mĩm ôm Vịt'),
(N'Gấu Bông Bánh Mì Capybara baby'),

(N'Gấu Bông SHIN nằm mặc đồ ngủ'),
(N'Gấu Bông Baby Three màu Galaxy'),

(N'Mèo Bông Mặt Quạo có mền 2in1'),
(N'Gấu Bông Capybara đội nón Sinh Nhật có mền 2in1'),
(N'Vịt Bông Vàng có mền 2in1'),

(N'Gấu Teddy mặc áo len Love'),
(N'Gấu Teddy heads and tales'),
(N'Gấu bông Teddy áo len Choco'),
(N'Gấu Bông Tốt Nghiệp – Gấu Teddy tốt nghiệp lông xù màu Nâu'),
(N'Gấu Bông Tốt Nghiệp – Gấu Teddy tốt nghiệp lông xù màu Vàng'),

(N'Mèo Hoàng Thượng Đội Lốt Thú Gối Ôm Dài'),
(N'Chó Bông Shiba lông Smooth Gối Ôm Dài'),
(N'Gấu Bông Lotso Nằm lông Smooth'),

(N'Gấu Teddy Angel Tím'),
(N'Heo Bông Tiktok cosplay Thỏ Hồng'),
(N'Gối ôm Mèo Nằm Ngửa Mặt Bựa'),
(N'Heo Bông má hồng nhỏ dãi có Mền 2in1'),
(N'Chó Bông husky gối ôm dài lông mịn'),
(N'Gối ôm Chim Cánh Cụt Bông lông mịn'),
(N'Gấu Bông Baby Three 4000% lông Layer Smooth Cao Cấp'),
(N'Vịt vàng Hello TikTok'),
(N'Gấu Bông rái cá chiên xù'),
(N'Voi bông trái cây'),
(N'Gấu Trúc Panda ngồi ôm Tre Xanh'),
(N'Gối mền Chó Bông Husky mặt ngáo'),
(N'Gấu Bông Hải Cẩu Sushi Cá Ngừ'),
(N'Dưa Hấu Bông Heo Tiktok'),
(N'Gấu Bông Trái Sầu Riêng'),
(N'Shin mặc quần vàng áo đỏ'),
(N'Gối mền 2in1 Gấu Bông Doremon ôm Bánh'),
(N'Gấu Bông Labubu form nằm có mền 2in1 – 55cm'),
(N'Gấu Bông Loopy nằm Cosplay Lotso Dâu'),
(N'Gấu Bông Lena cosplay Kuromi có mền 2in1'),
(N'Gấu Bông Kuromi tím cầm kem'),
(N'Gấu Bông Hello Kitty cosplay Stitch xanh'),
(N'Gối ôm Gấu Bông Melody Galaxy'),
(N'Thỏ Bông Cinnamoroll có mền 2in1'),
(N'Gối kê cổ Chữ U có nón – Thỏ Cinnamoroll'),
(N'Gấu Bông Bánh Kem Lotso có mền 2in1'),
(N'Gấu Bông Noel Gối ôm tròn đút tay Tuần Lộc Noel')

UPDATE Product 
SET Isenabled = 1
WHERE Isenabled is null

go
INSERT INTO Product_Category
values
(1, 'stitch'),
(1, 'sizenho'),
(2, 'stitch'),
(3, 'stitch'),
(3, 'goiom'),
(4, 'stitch'),
(5, 'hoa'),
(5, 'sizelon'),
(6, 'lotso'),
(6, 'hoa'),
(7, 'lena'),
(8, 'hoa'),
(8, 'sizelon'),
(9, 'vit'),
(10, 'huoucaoco'),
(11, 'capybara'),
(12, 'capybara'),
(12, 'goiom'),
(13, 'shin'),
(14, 'babythree'),
(15, 'meo'),
(15, '2in1'),
(16, 'capybara'),
(16, '2in1'),
(17, 'vit'),
(17, '2in1'),
(18, 'teddy'),
(19, 'teddy'),
(20, 'teddy'),
(21, 'teddy'),
(21, 'totnghiep'),
(22, 'teddy'),
(22, 'totnghiep'),
(23, 'meo'),
(23, 'goiom'),
(23, 'dai'),
(24, 'cho'),
(24, 'goiom'),
(24, 'dai'),
(25, 'lotso'),
(25, 'goiom'),
(26, 'teddy'),
(26, 'sizelon'),
(27, 'heo'),
(27, 'tho'),
(27, 'sizenho'),
(28, 'meo'),
(28, 'dai'),
(29, '2in1'),
(29, 'heo'),
(30, 'cho'),
(30, 'dai'),
(30, 'goiom'),
(31, 'goiom'),
(32, 'tho'),
(32, 'babythree'),
(33, 'vit'),
(33, 'sizenho'),
(34, 'raica'),
(35, 'voi'),
(35, 'traicaybong'),
(36, 'panda'),
(37, 'husky'),
(37, '2in1'),
(38, 'haicau'),
(39, 'duahau'),
(39, 'heo'),
(40, 'saurieng'),
(41, 'shin'),
(42, 'doraemon'),
(42, '2in1'),
(43, '2in1'),
(43, 'labubu'),
(44, 'lotso'),
(44, 'loopy'),
(44, 'sizelon'),
(45, 'kuromi'),
(45, 'lena'),
(45, '2in1'),
(46, 'kuromi'),
(47, 'stitch'),
(47, 'kitty'),
(48, 'melody'),
(48, 'goiom'),
(48, 'dai'),
(49, 'cinnamoroll'),
(50, 'goichuu'),
(50, 'cinnamoroll'),
(50, 'giare'),
(51, 'sinhnhat'),
(51, 'lotso'),
(51, '2in1'),
(52, 'noel')

go
INSERT INTO ProductSize
VALUES
(1, 45, 215000, 10),
(2, 95, 895000, 0),
(3, 30, 150000, 10),
(3, 50, 290000, 10),
(3, 60, 420000, 0),
(3, 80, 580000, 10),
(4, 35, 240000, 10),
(4, 45, 340000, 0),
(4, 55, 450000, 10),
(4, 70, 670000, 0),

(5, 40, 290000, 10),
(6, 40, 185000, 0),
(7, 30, 145000, 10),
(8, 40, 205000, 0),
(8, 70, 395000, 0),

(9, 60, 220000, 10),
(9, 85, 360000, 10),
(9, 150, 450000, 0),
(10, 45, 205000, 10),
(10, 65, 315000, 10),
(10, 80, 455000, 0),
(10, 120, 725000, 10),
(11, 35, 185000, 0),
(11, 45, 245000, 0),
(11, 55, 365000, 0),
(12, 20, 95000, 10),
(12, 70, 325000, 10),

(13, 55, 280000, 10),
(13, 75, 420000, 0),
(14, 30, 175000, 10),
(14, 60, 364000, 10),

(15, 45, 350000, 0),
(16, 45, 375000, 10),
(17, 40, 330000, 10),

(18, 55, 245000, 0),
(19, 80, 244000, 10),
(19, 160, 1251000, 10),
(19, 120, 1712000, 0),
(20, 40, 145000, 0),
(20, 50, 240000, 10),
(20, 75, 440000, 10),
(21, 40, 220000, 10),
(22, 40, 220000, 0),
(22, 50, 315000, 0),

(23, 80, 350000, 0),
(23, 100, 480000, 10),
(23, 125, 670000, 10),
(24, 60, 250000, 0),
(24, 80, 370000, 10),
(24, 110, 580000, 0),
(25, 110, 715000, 0),
(25, 60, 285000, 0),
(25, 80, 405000, 0);
go
INSERT INTO ProductSize
values
(26, 90, 410000, 0),
(26, 110, 590000, 0),
(27, 45, 295000, 2),
(27, 35, 205000, 3),
(28, 70, 230000, 2),
(29, 40, 340000, 1),
(30, 70, 210000, 1),
(30, 85, 290000, 1),
(31, 95, 385000, 1),
(32, 40, 175500, 2),
(32, 80, 607500, 2),
(33, 40, 165000, 4),
(34, 65, 275000, 3),
(34, 65, 395000, 5),
(35, 35, 210000, 2),
(36, 35, 130000, 1),
(37, 60, 320000, 5),
(38, 40, 165000, 10),
(39, 55, 190000, 2),
(39, 60, 285000, 2),
(40, 45, 340000, 1),
(41, 35, 160000, 5),
(42, 45, 330000, 8),
(43, 30, 330000, 0),
(44, 50, 195000, 0),
(44, 110, 635000, 0),
(45, 55, 370000, 2),
(46, 50, 265000, 0),
(46, 78, 405000,0),
(46, 100, 735000,0),
(47, 50, 215000, 0),
(48, 100, 440000, 3),
(48, 80, 310000, 4),
(49, 40, 370000, 3),
(50, 35, 215000, 4),
(51, 35, 350000, 2),
(52, 40, 288000, 4)

go
select * from ProductImages
INSERT INTO ProductImages
values
(1, 'gau-bong-stitch-hong-1-768x768.jpg'),
(2, 'gau-bong-stitch-cosplay-monster-5.jpg'),
(2, 'gau-bong-stitch-cosplay-monster-4.jpg'),
(2, 'gau-bong-stitch-cosplay-monster-3.jpg'),
(2, 'gau-bong-stitch-cosplay-monster-2.jpg'),
(2, 'gau-bong-stitch-cosplay-monster-1.jpg'),
(3, 'gau-bong-stitch-nam-long-min-smooth-tim-1.jpg'),
(3, 'gau-bong-stitch-nam-long-min-smooth-tim-2.jpg'),
(3, 'gau-bong-stitch-nam-long-min-smooth-tim-3.jpg'),
(3, 'gau-bong-stitch-nam-long-min-smooth-tim-4-768x768.jpg'),
(4, 'gau-bong-stitch-mac-yem-xanh-1.jpg'),

(5, 'gau-bong-hoa-hong-ngan-dung-qua-2.jpg'),
(5, 'gau-bong-hoa-hong-ngan-dung-qua-1.jpg'),
(6, 'hoa-gau-bong-lotso.jpg'),
(6, 'hoa-gau-bong-lotso-dau-2.jpg'),
(6, 'hoa-gau-bong-lotso-dau-1.jpg'),
(7, 'hoa-gau-bong-lena.jpg'),
(7, 'hoa-bong-gau-lena-mau-dam-1.jpg'),
(8, 'gau-bong-hoa-hong-khong-lo-8.jpg'),
(8, 'gau-bong-hoa-hong-khong-lo-7.jpg'),
(8, 'gau-bong-hoa-hong-khong-lo-3-768x768.jpg'),

(9, 'vit-bong-cosplay-bap-cai-xanh-1.jpg'),
(9, 'vit-bong-cosplay-bap-cai-xanh-2.jpg'),
(10, 'gau-bong-huou-cao-co-2.jpg'),
(10, 'gau-bong-huou-cao-co-1.jpg'),
(10, 'gau-bong-huou-cao-co-3.jpg'),
(11, 'gau-bong-capybara-om-vit-vang-1.jpg'),
(11, 'gau-bong-capybara-om-vit-vang-4.jpg'),
(11, 'gau-bong-capybara-om-vit-vang-3.jpg'),
(12, 'gau-bong-capybara-banh-mi-goi-om-dai-2.jpg'),
(12, 'gau-bong-capybara-banh-mi-goi-om-dai-1.jpg'),
(12, 'gau-bong-banh-mi-capybara-mini-1.jpg'),

(13, 'gau-bong-shin-mac-do-ngu-form-dai-3.jpg'),
(13, 'gau-bong-shin-mac-do-ngu-form-dai-2.jpg'),
(13, 'gau-bong-shin-mac-do-ngu-form-dai-1.jpg'),
(14, 'gaubong-baby-three-galaxy-3-1-768x768.jpg'),
(14, 'gaubong-baby-three-galaxy-1-1-768x768.jpg'),
(14, 'gaubong-baby-three-galaxy-2-768x768.jpg'),

(15, 'goi-men-meo-bong-mat-quao-1.jpg'),
(16, 'gau-bong-capybara-happy-birthday-co-men-2in1-1.jpg'),
(17, 'goi-men-vit-bong-vang-1.jpg'),

(18, 'gau-teddy-mac-ao-len-love-1.jpg'),
(19, 'gaubong-teddy-ao-len-choco-1m2.jpg'),
(19, 'gau-teddy-ao-len-choco-2022.jpg'),
(19, 'gau-teddy-choco-2m.jpg'),
(20, 'gau-teddy-heads-and-tales.jpg'),
(20, 'gau-bong-teddy-head-and-tales.jpg'),
(20, 'gau-teddy-head-and-tales-nau.jpg'),
(21, 'gaubong-gau-teddy-tot-nghiep-nau-1.jpg'),
(22, 'gaubong-teddy-tot-nghiep-2.jpg'),

(23, 'meo-bong-hoang-thuong-cosplay-tho-goi-om-dai-3.jpg'),
(23, 'meo-bong-hoang-thuong-cosplay-tho-goi-om-dai-2.jpg'),
(23, 'meo-bong-hoang-thuong-cosplay-panda-goi-om-dai-768x768.jpg'),
(24, 'cho-bong-shiba-smooth-goi-om-dai-3.jpg'),
(24, 'cho-bong-shiba-smooth-goi-om-dai-2.jpg'),
(24, 'cho-bong-shiba-smooth-goi-om-dai-1.jpg'),
(25, 'gaubong-lotso-goi-om-dai-2.jpg'),
(25, 'gaubong-lotso-goi-om-dai-1.jpg'),
(25, 'gau-bong-lotso-mau-do-nam-long-smooth-8.jpg');
go
INSERT INTO ProductImages
values
(26, 'gau-angel-tim.jpg'),
(26, 'gau-teddy-tim-angel.jpg'),
(26, 'teddy-tim-angel.jpg'),
(27, 'heo-bong-tktok-cosplay-tho-hong.jpg'),
(28, 'meo-bong-nam.jpg'),
(29, 'goi-men-2in1-heo-bong-nho-dai-3.jpg'),
(29, 'goi-men-2in1-heo-bong-nho-dai-2.jpg'),
(29, 'goi-men-2in1-heo-bong-nho-dai-1.jpg'),
(30, 'goi-om-gau-bong-cho-husky.jpg'),
(30, 'goiom-dai-thubong.jpg'),
(31, 'goi-om-chim-canh-cut-long-min-1.jpg'),
(32, 'gau-bong-baby-three-3.jpg'),
(32, 'gau-bong-baby-three-2.jpg'),
(32, 'gau-bong-baby-three-1.jpg'),
(33, 'vit-vang-gau-bong.jpg'),
(34, 'gau-bong-rai-ca-chien-xu-1.jpg'),
(34, 'gaubong-rai-ca-chien-xu-nau-1.jpg'),
(34, 'gau-bong-rai-ca-chien-xu.jpg'),
(35, 'voi-trai-cay-trai-dua-1.jpg'),
(35, 'voi-trai-cay-trai-thom.jpg'),
(35, 'voi-trai-cay-trai-x.jpg'),
(36, 'gau-truc-panda-om-cay-tre-xanh-3.jpg'),
(36, 'gau-truc-panda-om-cay-tre-xanh-2.jpg'),
(36, 'gau-truc-panda-om-cay-tre-xanh-1.jpg'),
(37, 'goi-men-2in1-cho-bong-husky-mat-ngao-ao-thun-den.jpg'),
(38, 'gau-bong-hai-cau-sushi-ca-ngu-2.jpg'),
(38, 'gau-bong-hai-cau-sushi-ca-ngu-1.jpg'),
(39, 'gau-bong-dua-hau-heo-tiktok.jpg'),
(39, 'dua-hau-bong-heo-tiktok.jpg'),
(40, 'gau-bong-trai-sau-rieng-2.jpg'),
(41, 'shin-quan-vang-ao-do-photo.jpg'),
(41, 'shin-quan-do-ao-vang.jpg'),
(42, 'goi-men-gau-bong-doremon-om-banh-1.jpg'),
(43, 'gau-bong-lalabubu-form-nam-co-men-2in1-nau-1.jpg'),
(43, 'gau-bong-lalabubu-form-nam-co-men-2in1-tim-1.jpg'),
(43, 'gau-bong-lalabubu-form-nam-co-men-2in1-hong-1.jpg'),
(44, 'gau-loopy-nam-cosplay-lotso-dau-3.jpg'),
(44, 'gau-loopy-nam-cosplay-lotso-dau-2.jpg'),
(44, 'gau-loopy-nam-cosplay-lotso-dau-1.jpg'),
(45, 'gau-bong-lena-cosplay-kuromi-co-men-2in1-1.jpg'),
(46, 'gaubong-kuromi-om-kem-1.jpg'),
(46, 'gau-bong-kuromi-cam-kem-6.jpg'),
(46, 'gau-bong-kuromi-cam-kem-4-768x768.jpg'),
(46, 'gau-bong-kuromi-cam-kem-3-768x768.jpg'),
(46, 'gau-bong-kuromi-cam-kem-2-768x768.jpg'),
(47, 'gau-bong-kitty-cosplay-stitch-xanh-1.jpg'),
(48, 'goi-om-gau-bong-melody-galaxy-1.jpg'),
(49, 'goi-men-2in1-tho-bong-Cinnamoroll-2.jpg'),
(49, 'goi-men-2in1-tho-bong-Cinnamoroll-1.jpg'),
(50, 'goi-chu-u-tho-cinna-1.jpg'),
(51, 'gau-bong-banh-kem-lotso-co-men-2in1-3.jpg'),
(51, 'gau-bong-banh-kem-lotso-co-men-2in1-2.jpg'),
(51, 'gau-bong-banh-kem-lotso-co-men-2in1-1.jpg'),
(52, 'gaubong-ong-tuan-loc-noel-goi-om-dut-tay-2.jpg'),
(52, 'gaubong-ong-tuan-loc-noel-goi-om-dut-tay-3.jpg'),
(52, 'gaubong-ong-tuan-loc-noel-goi-om-dut-tay-5-768x768.jpg')
go

go
INSERT INTO Discount (ProductID, DiscountName, StartDate, EndDate, DiscountRate)
VALUES
(1, N'Giảm giá 20/11', GETDATE(), DATEADD(month, 1, GETDATE()), 15),
(5, N'Tri ân Nhà giáo', GETDATE(), DATEADD(month, 1, GETDATE()), 20),
(8, N'Giảm giá 20/11', GETDATE(), DATEADD(month, 1, GETDATE()), 10),
(10, N'Giảm giá Black Friday', GETDATE(), DATEADD(month, 1, GETDATE()), 50),
(12, N'Giảm giá Black Friday', GETDATE(), DATEADD(month, 1, GETDATE()), 40),
(15, N'Giảm giá Black Friday', GETDATE(), DATEADD(month, 1, GETDATE()), 30),
(18, N'Xả hàng cuối năm', GETDATE(), DATEADD(month, 1, GETDATE()), 70),
(20, N'Chào Giáng Sinh', GETDATE(), DATEADD(month, 1, GETDATE()), 25),
(22, N'Chào Giáng Sinh', GETDATE(), DATEADD(month, 1, GETDATE()), 25),
(25, N'Chào Giáng Sinh', GETDATE(), DATEADD(month, 1, GETDATE()), 30),
(27, N'Xả hàng cuối năm', GETDATE(), DATEADD(month, 1, GETDATE()), 60),
(30, N'Giảm giá 20/11', GETDATE(), DATEADD(month, 1, GETDATE()), 10),
(33, N'Tri ân Nhà giáo', GETDATE(), DATEADD(month, 1, GETDATE()), 15),
(35, N'Giảm giá Black Friday', GETDATE(), DATEADD(month, 1, GETDATE()), 50),
(38, N'Chào Giáng Sinh', GETDATE(), DATEADD(month, 1, GETDATE()), 20),
(40, N'Xả hàng cuối năm', GETDATE(), DATEADD(month, 1, GETDATE()), 75),
(42, N'Giảm giá 20/11', GETDATE(), DATEADD(month, 1, GETDATE()), 10),
(45, N'Giảm giá Black Friday', GETDATE(), DATEADD(month, 1, GETDATE()), 40),
(48, N'Chào Giáng Sinh', GETDATE(), DATEADD(month, 1, GETDATE()), 30),
(50, N'Xả hàng cuối năm', GETDATE(), DATEADD(month, 1, GETDATE()), 80)
go

go
INSERT INTO ROLES
values
('admin'), --1
('user') --2
go

go
INSERT INTO Users
values
(1, N'Nguyễn Thành Tài', '0933733336', 'admin123', 'ntai8448@gmail.com'), --1
(2, N'Bích Ngọc', '0123456789', 'ngoc123', 'ngoc22@gmail.com'), --2
(2, N'Minh Nguyệt', '024681012', 'nguyet123', 'nguyet33@gmail.com') --3
go
SELECT * FROM Users

go
INSERT INTO Rating (ProductID, UserID, Star, Comment, ProductSizeName, RatingDate)
VALUES
(1, 2, 5, N'Stitch hồng cưng xỉu, lông mịn xịn xò.', 45, GETDATE()),
(2, 3, 5, N'Đỉnh chóp, cosplay monster chất lừ.', 95, GETDATE()),
(3, 2, 4, N'Nhìn lười y chang tui, oke la.', 50, GETDATE()),
(4, 3, 5, N'Em này mặc yếm iu hết nước chấm.', 45, GETDATE()),
(5, 2, 5, N'Tặng gấu bông hoa hồng này là uy tín luôn, bồ tui mê.', 40, GETDATE()),
(6, 3, 3, N'Hoa dâu cũng tạm, hơi ít bông.', 40, GETDATE()),
(7, 2, 4, N'Bó này xinh, tặng 8/3 là hết bài.', 30, GETDATE()),
(8, 3, 5, N'To vãi chưởng, bồ tui ôm k hết.', 70, GETDATE()),
(9, 2, 5, N'Vịt súp lơ nhìn hề vãi =))', 60, GETDATE()),
(10, 3, 4, N'Lông mịn thật sự, sờ sướng tay.', 65, GETDATE()),
(11, 2, 5, N'Capybara mặt bình thản iu xỉu, ôm thêm con vịt nữa.', 45, GETDATE()),
(12, 3, 5, N'Nhìn mlem mlem, muốn cắn.', 70, GETDATE()),
(13, 2, 5, N'Shin mông chổng lên trời, 10 điểm.', 55, GETDATE()),
(14, 3, 2, N'Màu galaxy hơi phèn, k đẹp như ảnh.', 30, GETDATE()),
(15, 2, 5, N'Mặt quạo y tui, mền xịn.', 45, GETDATE()),
(16, 3, 5, N'Vừa làm gấu vừa có mền, 10 điểm, capy cute.', 45, GETDATE()),
(17, 2, 4, N'Mền hơi mỏng, đc cái vịt xinh.', 40, GETDATE()),
(18, 3, 5, N'Classic, gấu teddy tặng ai cũng mê.', 55, GETDATE()),
(19, 2, 1, N'Lông rụng quá, toang.', 80, GETDATE()),
(20, 3, 4, N'Con này oke, ổn áp.', 50, GETDATE()),
(21, 2, 5, N'Mua tặng tốt nghiệp, cháy thật sự.', 40, GETDATE()),
(22, 3, 5, N'Bạn tui nhận quà mà iu xỉu.', 40, GETDATE()),
(23, 2, 4, N'Gối ôm dài, ngủ bao phẻ.', 80, GETDATE()),
(24, 3, 5, N'Mặt ngáo, lông mịn, ôm đã.', 60, GETDATE()),
(25, 2, 5, N'Mê chữ ê kéo dài, thơm mùi dâu.', 60, GETDATE()),
(26, 3, 3, N'Màu tím hơi sến.', 90, GETDATE()),
(27, 2, 5, N'Heo đội nón thỏ, cưng xỉu.', 35, GETDATE()),
(28, 3, 5, N'Nhìn mặt báo k chịu đc, hề hước.', 70, GETDATE()),
(29, 2, 4, N'Vừa ôm vừa đắp mền, tiện.', 40, GETDATE()),
(30, 3, 5, N'Mặt ngáo uy tín, lông mịn.', 70, GETDATE()),
(31, 2, 4, N'Ôm mát tay, gối ôm cánh cụt này đc.', 95, GETDATE()),
(32, 3, 5, N'Con này xịn xò, lông layer đẹp, đắt xắt ra miếng.', 40, GETDATE()),
(33, 2, 3, N'Trend tiktok thôi chứ bt, lông k đẹp lắm.', 40, GETDATE()),
(34, 3, 5, N'Nhìn xù xù mà cưng, rái cá chất.', 65, GETDATE()),
(35, 2, 4, N'Voi dâu, voi bơ... ý tưởng hay, cưng.', 35, GETDATE()),
(36, 3, 5, N'Panda auto 10 điểm, k nói nhiều.', 35, GETDATE()),
(37, 2, 5, N'Lại là mền 2in1, mặt ngáo, ưng.', 60, GETDATE()),
(38, 3, 4, N'Nhìn mlem quá, hải cẩu sushi.', 40, GETDATE()),
(39, 2, 5, N'Heo dưa hấu, con này cháy nhất shop.', 55, GETDATE()),
(40, 3, 1, N'Gai nhọn, ôm í ẹ, k thích lắm.', 45, GETDATE()),
(41, 2, 5, N'Shin này huyền thoại rồi, cute.', 35, GETDATE()),
(42, 3, 5, N'Fan Mon, mền ấm, uy tín.', 45, GETDATE()),
(43, 2, 5, N'Labubu đang hot, có mền nữa, đỉnh.', 55, GETDATE()),
(44, 3, 4, N'Loopy dâu, 2 trong 1, cũng đc.', 50, GETDATE()),
(45, 2, 5, N'Kuromi 100 điểm, mền xịn.', 55, GETDATE()),
(46, 3, 5, N'Con này chất lừ, kuromi mãi đỉnh.', 50, GETDATE()),
(47, 2, 2, N'Kết hợp hơi quê, k hợp.', 50, GETDATE()),
(48, 3, 5, N'Gối ôm melody màu galaxy đẹp, iu xỉu.', 80, GETDATE()),
(49, 2, 5, N'Mền 2in1, Cinnamoroll cưnggg.', 40, GETDATE()),
(50, 3, 4, N'Đi xe tiện, mà hơi nóng cổ.', 35, GETDATE()),
(51, 2, 5, N'Lotso thơm, mền xịn, hết nước chấm.', 35, GETDATE()),
(52, 3, 5, N'Mua tặng noel uy tín luôn, đút tay ấm.', 40, GETDATE()),
(1, 3, 4, N'Cũng đc, mà màu hồng hơi lợt.', 45, GETDATE()),
(2, 2, 1, N'Xấu, cosplay í ẹ, không giống hình.', 95, GETDATE()),
(5, 3, 5, N'Tặng sinh nhật oke la, giấu quà tiện.', 40, GETDATE()),
(11, 3, 5, N'Capy mặt bất biến, cưng vãi.', 45, GETDATE()),
(15, 3, 5, N'Mền ấm, mặt mèo bao quạo.', 45, GETDATE()),
(16, 2, 5, N'Đỉnh chóp, mua tặng SN là lemỏn.', 45, GETDATE()),
(21, 3, 5, N'Mua cho em gái, nó mê.', 40, GETDATE()),
(24, 2, 5, N'Gối ôm to, ngủ sướng, bao phẻ.', 80, GETDATE()),
(25, 3, 5, N'Thơm mùi dâu, iu xỉu, ôm đã.', 80, GETDATE()),
(28, 2, 5, N'Nhìn mặt báo đời =)) hài.', 70, GETDATE()),
(30, 2, 4, N'Lông mượt, ôm êm, giao hàng nhanh.', 85, GETDATE()),
(32, 2, 5, N'Con này cháy nhất shop, lông mượt thật.', 80, GETDATE()),
(36, 2, 5, N'Panda thì k chê đc, 10 điểm.', 35, GETDATE()),
(40, 2, 5, N'Fan sầu riêng, con này 10 điểm, nhìn cưng.', 45, GETDATE()),
(43, 3, 5, N'Labubu 2in1, hốt lẹ còn kịp.', 55, GETDATE()),
(45, 3, 5, N'Kuromi 2in1, hết nước chấm, mền to.', 55, GETDATE()),
(46, 2, 5, N'Chất, mua cho bồ, ưng ý.', 78, GETDATE()),
(48, 2, 4, N'Màu đẹp, hơi to so với giường.', 100, GETDATE()),
(49, 3, 5, N'Mền xịn, gấu cưng, 10 điểm.', 40, GETDATE()),
(51, 3, 5, N'Lotso thơm, mền 2in1 tiện, hết bài.', 35, GETDATE()),
(52, 2, 5, N'Ôm đút tay ấm, mùa đông đỉnh.', 40, GETDATE()),
(1, 2, 3, N'Bình thường, lông cũng k mịn lắm.', 45, GETDATE()),
(2, 3, 4, N'Cũng chất, con tui thích.', 95, GETDATE()),
(11, 2, 5, N'Capybara uy tín nhất hệ mặt trời.', 55, GETDATE()),
(12, 2, 4, N'Nhìn đói bụng ghê, cưng.', 20, GETDATE()),
(13, 3, 5, N'Mông xinh =)), mua vì cái mông.', 75, GETDATE()),
(15, 2, 5, N'Mền ấm, mặt mèo chuẩn quạu, 10 điểm.', 45, GETDATE()),
(19, 3, 5, N'Lông mềm, k rụng. oke la.', 120, GETDATE()),
(24, 3, 5, N'Ôm ngủ hết bài, lông mướt.', 110, GETDATE()),
(25, 2, 1, N'Mùi dâu hắc quá, toang.', 110, GETDATE()),
(28, 3, 5, N'Trùm báo =)), nhìn mặt là mắc cười.', 70, GETDATE()),
(30, 2, 5, N'Đẹp, lông mịn, uy tín.', 70, GETDATE()),
(36, 3, 4, N'Panda oke, giao nhanh.', 35, GETDATE()),
(44, 2, 5, N'Loopy hết nước chấm, cưng vãi.', 110, GETDATE()),
(46, 3, 5, N'10 điểm, kuromi chất.', 50, GETDATE()),
(48, 2, 5, N'Iu em này, gối ôm to, màu đẹp.', 80, GETDATE()),
(49, 3, 4, N'Tiện, nhưng mền hơi nhỏ.', 40, GETDATE()),
(51, 2, 5, N'Thơm, xịn, 2in1 quá tiện.', 35, GETDATE());
go
go
--DELETE FROM RATING
go
SELECT * FROM Rating
SELECT * FROM Product_Category
select * from ShoppingCart
SELECT * FROM Users
Select * From Orders

select OrderID, ProductName from OrderDetail od, ProductSize ps, Product pd
WHERE od.ProductSizeID = ps.ProductSizeID and ps.ProductID = pd.ProductID

SELECT * FROM ProductSize where ProductID = 1
Select * from ProductImages where ProductID = 1
select * from Product where ProductID = 1

----------Admin-----------------
SP_ADDROLE N'Admin'
ALTER ROLE db_owner ADD MEMBER [Admin];

SP_ADDLOGIN 'tai', '123'
SP_ADDUSER 'tai', 'tai'
SP_ADDROLEMEMBER [Admin], 'tai'


SP_ADDROLE 'Nhân viên'

SELECT IS_ROLEMEMBER('Admin');


create table AppUser
(
	ID int identity(1,1),
	FirstName nvarchar(20),
	LastName nvarchar(20),
	Email nvarchar(30),
	Password nvarchar(30),

	constraint PK_AppUser primary key (ID),
	constraint UQ_email unique (email)

)

create table Todos
(
	ID int identity(1,1),
	UserID int, 
	Content nvarchar(200),

	constraint PK_Todo primary key(ID),
	constraint FK_User foreign key(UserID) references AppUser(ID)

)

create table DoneTodos
(
	ID int identity(1,1),
	UserID int, 
	Content nvarchar(200),

	constraint PK_DoneTodos primary key(ID),
	constraint FK_User_DoneTodo foreign key(UserID) references AppUser(ID)
)



create procedure proc_insert_user
(
	@firstName nvarchar(20),
	@lastName nvarchar(20),
	@email nvarchar(30),
	@password nvarchar(30)
)
as
begin
	insert into AppUser 
	values(@firstName,@lastName,@email,@password)
end
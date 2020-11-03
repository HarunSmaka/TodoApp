create procedure proc_get_userID
(
	@email nvarchar(30)
)
as
begin
	select id 
	from AppUser
	where Email = @email
end
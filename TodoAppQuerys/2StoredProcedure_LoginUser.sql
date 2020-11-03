create procedure proc_LoginUser
(
	@email nvarchar(30),
	@password nvarchar(30)
)
as
begin
select *
from AppUser
where Email = @email and Password = @password
end
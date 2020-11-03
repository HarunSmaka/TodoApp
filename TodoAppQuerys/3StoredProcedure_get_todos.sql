create procedure proc_get_todos
(
	@email nvarchar(30)
)
as
begin
select t.Content
from AppUser u join Todos t
on u.ID = t.UserID
where u.Email = @email
end

create procedure proc_get_done_todos
(
	@userID int
)
as
begin
select dt.Content
from AppUser u join DoneTodos dt
on u.ID = dt.UserID
where u.ID = @userID
end
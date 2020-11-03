create procedure proc_email_search
(
	@email nvarchar(30)
)
as
begin
select email 
from AppUser
where email = @email
end
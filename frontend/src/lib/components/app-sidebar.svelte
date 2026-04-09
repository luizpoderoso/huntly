<script lang="ts">
	import { resolve } from '$app/paths';
	import { authStore } from '$lib/stores/auth.svelte';
	import * as Sidebar from '$lib/components/ui/sidebar';
	import { Separator } from '$lib/components/ui/separator';
	import { goto } from '$app/navigation';

	function handleLogout() {
		authStore.logout();
		goto(resolve('/login'));
	}
</script>

<Sidebar.Root>
	<Sidebar.Header>
		<div class="flex flex-col gap-1 px-2 py-2">
			<span class="text-lg font-bold">Huntly</span>
			<span class="text-sm text-muted-foreground">{authStore.user?.username}</span>
		</div>
	</Sidebar.Header>

	<Separator />

	<Sidebar.Content>
		<Sidebar.Group>
			<Sidebar.GroupLabel>Navigation</Sidebar.GroupLabel>
			<Sidebar.GroupContent>
				<Sidebar.Menu>
					<Sidebar.MenuItem class="*:hover:cursor-pointer">
						<Sidebar.MenuButton>
							<a href={resolve('/dashboard')}>Dashboard</a>
						</Sidebar.MenuButton>
					</Sidebar.MenuItem>
				</Sidebar.Menu>
			</Sidebar.GroupContent>
		</Sidebar.Group>
	</Sidebar.Content>

	<Sidebar.Footer>
		<Separator />
		<button
			onclick={handleLogout}
			class="w-full px-4 py-3 text-left text-sm transition-colors hover:cursor-pointer hover:bg-accent"
		>
			Sign out
		</button>
	</Sidebar.Footer>
</Sidebar.Root>

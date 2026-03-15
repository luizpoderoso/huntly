<script lang="ts">
    import { goto } from '$app/navigation';
    import { resolve } from '$app/paths';
    import { login } from '$lib/api/auth';
    import { authStore } from '$lib/stores/auth.svelte';
    import { Button } from '$lib/components/ui/button';
    import { Input } from '$lib/components/ui/input';
    import { Label } from '$lib/components/ui/label';
    import * as Card from '$lib/components/ui/card';

    let email = $state('');
    let password = $state('');
    let loading = $state(false);
    let error = $state<string | null>(null);

    async function handleSubmit() {
        loading = true;
        error = null;
        try {
            const response = await login({ email, password });
            authStore.login(response);
            await goto(resolve('/dashboard'));
        } catch (e: unknown) {
            error = (e as { error: string })?.error ?? 'Something went wrong.';
        } finally {
            loading = false;
        }
    }
</script>

<Card.Root>
    <Card.Header class="text-center">
        <Card.Title class="text-2xl">Welcome back</Card.Title>
        <Card.Description>Sign in to your Huntly account</Card.Description>
    </Card.Header>

    <Card.Content>
        <form onsubmit={handleSubmit} class="flex flex-col gap-4">
            {#if error}
                <p class="text-destructive text-sm text-center">{error}</p>
            {/if}

            <div class="flex flex-col gap-2">
                <Label for="email">Email</Label>
                <Input
                    id="email"
                    type="email"
                    bind:value={email}
                    placeholder="you@example.com"
                />
            </div>

            <div class="flex flex-col gap-2">
                <Label for="password">Password</Label>
                <Input
                    id="password"
                    type="password"
                    bind:value={password}
                    placeholder="••••••••"
                />
            </div>

            <Button type="submit" disabled={loading} class="w-full">
                {loading ? 'Signing in...' : 'Sign in'}
            </Button>
        </form>
    </Card.Content>

    <Card.Footer class="justify-center">
        <p class="text-muted-foreground text-sm">
            Don't have an account?
            <a href={resolve('/register')} class="text-primary underline-offset-4 hover:underline">
                Register
            </a>
        </p>
    </Card.Footer>
</Card.Root>